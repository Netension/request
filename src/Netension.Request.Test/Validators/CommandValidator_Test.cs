using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Validators;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Validators
{
    public class CommandValidator_Test
    {
        private Mock<ICommandHandler<Command>> _commandHandlerMock;
        private List<IValidator<Command>> _validators;
        private readonly ILogger<CommandValidator<Command>> _logger;

        public CommandValidator_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<CommandValidator<Command>>();
        }

        private CommandValidator<Command> CreateSUT()
        {
            _commandHandlerMock = new Mock<ICommandHandler<Command>>();
            _validators = new List<IValidator<Command>>();

            return new CommandValidator<Command>(_validators, _commandHandlerMock.Object, _logger);
        }

        [Fact(DisplayName = "CommandValidator - HandleAsync - Validate command")]
        public async Task CommandValidator_HandleAsnyc_ValidateCommmand()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();
            var validatorMock = new Mock<IValidator<Command>>();

            _validators.Add(validatorMock.Object);
            _validators.Add(validatorMock.Object);

            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            await sut.HandleAsync(command, default).ConfigureAwait(false);

            // Assert
            validatorMock.Verify(v => v.ValidateAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "CommandValidator - HandleAsync - Throw ValidationException")]
        public async Task CommandValidator_HandleAsnyc_ThrowValidationException()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();
            var validatorMock = new Mock<IValidator<Command>>();

            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new Fixture().CreateMany<ValidationFailure>(2)));

            _validators.Add(validatorMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await sut.HandleAsync(command, default).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "CommandValidator - HandleAsync - Validator not found")]
        public async Task CommandValidator_HandleAsnyc_ValidatorNotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            // Act
            await sut.HandleAsync(command, default).ConfigureAwait(false);

            // Assert
            _commandHandlerMock.Verify(ch => ch.HandleAsync(It.Is<Command>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
