using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Blazor.Handlers;
using Netension.Request.Blazor.Senders;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class CommandExceptionHandlerMiddleware_Test
    {
        private readonly ILogger<CommandExceptionHandlerMiddleware> _logger;
        private Mock<ICommandSender> _commandSenderMock;
        private Mock<IErrorHandler> _errorHandlerMock;

        public CommandExceptionHandlerMiddleware_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<CommandExceptionHandlerMiddleware>();
        }

        private CommandExceptionHandlerMiddleware CreateSUT()
        {
            _commandSenderMock = new Mock<ICommandSender>();
            _errorHandlerMock = new Mock<IErrorHandler>();

            return new CommandExceptionHandlerMiddleware(_commandSenderMock.Object, _errorHandlerMock.Object, _logger);
        }

        [Fact(DisplayName = "[BLAZOR-EHM001][Command]: Handle Server error")]
        [Trait("Technology", "Blazor")]
        public async Task Blazor_Commmand_HandleException()
        {
            // Arrange
            var sut = CreateSUT();

            _commandSenderMock.Setup(cs => cs.SendAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            await sut.SendAsync(new Command(), default);

            // Assert
            _errorHandlerMock.Verify(eh => eh.HandleServerErrorAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[BLAZOR-EHM002][Command]: Handle VerificationException")]
        [Trait("Technology", "Blazor")]
        public async Task Blazor_Commmand_HandleVerificationException()
        {
            // Arrange
            var sut = CreateSUT();

            var errorCode = new Fixture().Create<int>();
            var message = new Fixture().Create<string>();

            _commandSenderMock.Setup(cs => cs.SendAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new VerificationException(errorCode, message));

            // Act
            await sut.SendAsync(new Command(), default);

            // Assert
            _errorHandlerMock.Verify(eh => eh.HandleVerificationErrorAsync(It.Is<int>(ec => ec == errorCode), It.Is<string>(m => m.Equals(message)), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[BLAZOR-EHM003][Command]: Handle ValidationException")]
        [Trait("Technology", "Blazor")]
        public async Task Blazor_Commmand_HandleValidationException()
        {
            // Arrange
            var sut = CreateSUT();

            var validationResults = new Fixture().CreateMany<ValidationFailure>();

            _commandSenderMock.Setup(cs => cs.SendAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationResults));

            // Act
            await sut.SendAsync(new Command(), default);

            // Assert
            _errorHandlerMock.Verify(eh => eh.HandleValidationErrorAsync(It.Is<IEnumerable<ValidationFailure>>(vf => vf.Equals(validationResults)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
