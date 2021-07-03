using AutoFixture;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Netension.Request.Blazor.Brokers;
using Netension.Request.Blazor.ValueObjects;
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

        [Fact(DisplayName = "[BLAZOR-ERB001]: Publish server error")]
        [Trait("Technology", "Blazor")]
        [Trait("Feature", "Error Handling")]
        public async Task PublishServerError()
        {
            // Arrange
            var sut = CreateSUT();
            var handled = false;

            sut.Subscribe((_, _) => { handled = true; return Task.CompletedTask; });

            // Act
            await sut.PublishAsync(default);

            // Assert
            Assert.True(handled);
        }

        [Fact(DisplayName = "[BLAZOR-ERB002]: Publish verification error")]
        [Trait("Technology", "Blazor")]
        [Trait("Feature", "Error Handling")]
        public async Task PublishVerificationError()
        {
            // Arrange
            var sut = CreateSUT();
            var error = new Fixture().Create<Error>();

            Error result = null;

            sut.Subscribe((error, _) => { result = error; return Task.CompletedTask; });

            // Act
            await sut.PublishAsync(error.Code, error.Message, default);

            // Assert
            Assert.Equal(error.Code, result.Code);
            Assert.Equal(error.Message, result.Message);
        }

        [Fact(DisplayName = "[BLAZOR-ERB001]: Publish validation error")]
        [Trait("Technology", "Blazor")]
        [Trait("Feature", "Error Handling")]
        public async Task PublishValidationError()
        {
            // Arrange
            var sut = CreateSUT();
            var failures = new Fixture().CreateMany<ValidationFailure>(3);

            Error result = null;

            sut.Subscribe((error, _) => { result = error; return Task.CompletedTask; });

            // Act
            await sut.PublishAsync(failures, default);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Validation exception", result.Message);
        }

        [Fact(DisplayName = "[BLAZOR-ERB004]: Unsubscribe")]
        [Trait("Technology", "Blazor")]
        [Trait("Feature", "Error Handling")]
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
