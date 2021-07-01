using Microsoft.Extensions.Logging;
using Moq;
using Netension.Extensions.Correlation;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Http.Wrappers;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Wrappers
{
    public class HttpCorrelationWrapper_Test
    {
        private readonly ILogger<HttpCorrelationWrapper> _logger;
        private Mock<ICorrelationAccessor> _correlationContextMock;
        private Mock<IHttpRequestWrapper> _httpRequestWrapperMock;

        public HttpCorrelationWrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<HttpCorrelationWrapper>();
        }

        private HttpCorrelationWrapper CreateSUT()
        {
            _correlationContextMock = new Mock<ICorrelationAccessor>();
            _httpRequestWrapperMock = new Mock<IHttpRequestWrapper>();

            return new HttpCorrelationWrapper(_correlationContextMock.Object, _httpRequestWrapperMock.Object, _logger);
        }

        [Fact(DisplayName = "HttpCorrelationWrapper - WrapAsync - Call next wrapper")]
        public async Task HttpCorrelationWrapper_WrapAsync_CallNextWrapper()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            _httpRequestWrapperMock.Setup(hrw => hrw.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonContent.Create(command));

            _correlationContextMock.SetupGet(cc => cc.CorrelationId)
                .Returns(Guid.NewGuid());
            _correlationContextMock.SetupGet(cc => cc.MessageId)
                .Returns(Guid.NewGuid());

            // Act
            await sut.WrapAsync(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            _httpRequestWrapperMock.Verify(hrw => hrw.WrapAsync(It.Is<IRequest>(r => r.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HttpCorrelationWrapper - WrapAsync - Add Correlation-Id header")]
        public async Task HttpCorrelationWrapper_WrapAsync_AddCorrelationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var correlationId = Guid.NewGuid();
            var command = new Command();

            _httpRequestWrapperMock.Setup(hrw => hrw.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonContent.Create(command));

            _correlationContextMock.SetupGet(cc => cc.CorrelationId)
                .Returns(correlationId);
            _correlationContextMock.SetupGet(cc => cc.MessageId)
                .Returns(Guid.NewGuid());

            // Act
            var request = await sut.WrapAsync(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.Equal(correlationId, request.Headers.GetCorrelationId());
        }

        [Fact(DisplayName = "HttpCorrelationWrapper - WrapAsync - Add Causation-Id header")]
        public async Task HttpCorrelationWrapper_WrapAsync_AddCausationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var messageId = Guid.NewGuid();
            var command = new Command();

            _httpRequestWrapperMock.Setup(hrw => hrw.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonContent.Create(command));

            _correlationContextMock.SetupGet(cc => cc.CorrelationId)
                .Returns(Guid.NewGuid());
            _correlationContextMock.SetupGet(cc => cc.MessageId)
                .Returns(messageId);

            // Act
            var request = await sut.WrapAsync(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.Equal(messageId, request.Headers.GetCausationId());
        }
    }
}
