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
    public class QueryExceptionHandlerMiddleware_Test
    {
        private readonly ILogger<QueryExceptionHandlerMiddleware> _logger;
        private Mock<IQuerySender> _querySenderMock;
        private Mock<IErrorHandler> _errorHandlerMock;

        public QueryExceptionHandlerMiddleware_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<QueryExceptionHandlerMiddleware>();
        }
        private QueryExceptionHandlerMiddleware CreateSUT()
        {
            _querySenderMock = new Mock<IQuerySender>();
            _errorHandlerMock = new Mock<IErrorHandler>();

            return new QueryExceptionHandlerMiddleware(_querySenderMock.Object, _errorHandlerMock.Object, _logger);
        }

        [Fact(DisplayName = "[BLAZOR-EHM001][Query]: Handle Server Error")]
        [Trait("Technology", "Blazor")]
        public async Task Blazor_Query_HandleServerError()
        {
            // Arrange
            var sut = CreateSUT();

            _querySenderMock.Setup(qs => qs.QueryAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            await sut.QueryAsync(new Query<object>(), default);

            // Assert
            _errorHandlerMock.Verify(eh => eh.HandleServerErrorAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[BLAZOR-EHM001][Query]: Handle Verification error")]
        [Trait("Technology", "Blazor")]
        public async Task Blazor_Query_HandleVerificationError()
        {
            // Arrange
            var sut = CreateSUT();

            var errorCode = new Fixture().Create<int>();
            var message = new Fixture().Create<string>();

            _querySenderMock.Setup(qs => qs.QueryAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new VerificationException(errorCode, message));

            // Act
            await sut.QueryAsync(new Query<object>(), default);

            // Assert
            _errorHandlerMock.Verify(eh => eh.HandleVerificationErrorAsync(It.Is<int>(ec => ec == errorCode), It.Is<string>(m => m.Equals(message)), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[BLAZOR-EHM001][Query]: Handle Validation error")]
        [Trait("Technology", "Blazor")]
        public async Task Blazor_Query_HandleValidationError()
        {
            // Arrange
            var sut = CreateSUT();

            var failures = new Fixture().Create<IEnumerable<ValidationFailure>>();

            _querySenderMock.Setup(qs => qs.QueryAsync(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(failures));

            // Act
            await sut.QueryAsync(new Query<object>(), default);

            // Assert
            _errorHandlerMock.Verify(eh => eh.HandleValidationErrorAsync(It.Is<IEnumerable<ValidationFailure>>(vf => vf.Equals(failures)), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
