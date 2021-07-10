using AutoFixture;
using Microsoft.Extensions.Logging;
using Netension.Request.Blazor.Brokers;
using Netension.Request.Blazor.ValueObjects;
using Netension.Request.Http.Enumerations;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test
{
    public class ErrorBroker_Test
    {
        private readonly ILogger<ErrorBroker> _logger;

        public ErrorBroker_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<ErrorBroker>();
        }

        private ErrorBroker CreateSUT()
        {
            return new ErrorBroker(_logger);
        }

        [Fact(DisplayName = "[BALZOR-ERB001]: Publish server error")]
        [Trait("Technology", "Blazor")]
        [Trait("Feature", "EH - Error Handling")]
        public async Task PublishServerError()
        {
            // Arrange
            var sut = CreateSUT();
            Error result = null;

            sut.Subscribe((error, _) => { result = error; return Task.CompletedTask; });

            // Act
            await sut.PublishAsync(default);

            // Assert
            var typedResult = Assert.IsType<InternalServerError>(result);
            Assert.Equal(ErrorCodeEnumeration.InternalServerError.Id, typedResult.Code);
            Assert.Equal(ErrorCodeEnumeration.InternalServerError.Message, typedResult.Message);
        }

        [Fact(DisplayName = "[BLAZOR-ERB002]: Publish verification error")]
        [Trait("Technology", "Blazor")]
        [Trait("Feature", "EH - Error Handling")]
        public async Task PublishVerificationError()
        {
            // Arrange
            var sut = CreateSUT();
            var error = new Fixture().Create<VerificationError>();

            Error result = null;

            sut.Subscribe((error, _) => { result = error; return Task.CompletedTask; });

            // Act
            await sut.PublishAsync(error.Code, error.Message, default);

            // Assert
            var typedResult = Assert.IsType<VerificationError>(result);
            Assert.Equal(error.Code, typedResult.Code);
            Assert.Equal(error.Message, typedResult.Message);
        }

        [Fact(DisplayName = "[BLAZOR-ERB003]: Publish validation error")]
        [Trait("Technology", "Blazor")]
        [Trait("Feature", "EH - Error Handling")]
        public async Task PublishValidationError()
        {
            // Arrange
            var sut = CreateSUT();
            var error = new Fixture().Create<ValidationError>();

            Error result = null;

            sut.Subscribe((error, _) => { result = error; return Task.CompletedTask; });

            // Act
            await sut.PublishAsync(error.Failures, default);

            // Assert
            var typedResult = Assert.IsType<ValidationError>(result);
            Assert.Equal(ErrorCodeEnumeration.ValidationFailed.Id, typedResult.Code);
            Assert.Equal(ErrorCodeEnumeration.ValidationFailed.Message, typedResult.Message);
            Assert.Equal(error.Failures, typedResult.Failures);
        }

        [Fact(DisplayName = "[BLAZOR-ERB004]: Unsubscribe")]
        [Trait("Technology", "Blazor")]
        [Trait("Feature", "EH - Error Handling")]
        public async Task Unsubscribe()
        {
            // Arrange
            var sut = CreateSUT();
            var handleCount = 0;
            Task handler(Error error, System.Threading.CancellationToken cancellationToken) { handleCount++; return Task.CompletedTask; }

            sut.Subscribe(handler);
            await sut.PublishAsync(default);

            // Act
            sut.Unsubscribe(handler);
            await sut.PublishAsync(default);

            // Assert
            Assert.Equal(1, handleCount);
        }
    }
}
