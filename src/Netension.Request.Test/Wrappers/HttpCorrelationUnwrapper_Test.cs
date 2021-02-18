using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Netension.Extensions.Correlation;
using Netension.Extensions.Correlation.Defaults;
using Netension.Request.NetCore.Asp.Unwrappers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Wrappers
{
    public class HttpCorrelationUnwrapper_Test
    {
        private readonly ILogger<HttpCorrelationUnwrapper> _logger;
        private Mock<ICorrelationMutator> _correlationMutatorMock;
        private Mock<IHttpRequestUnwrapper> _httpRequestUnwrapperMock;

        public HttpCorrelationUnwrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<HttpCorrelationUnwrapper>();
        }

        private HttpCorrelationUnwrapper CreateSUT()
        {
            _correlationMutatorMock = new Mock<ICorrelationMutator>();
            _httpRequestUnwrapperMock = new Mock<IHttpRequestUnwrapper>();

            return new HttpCorrelationUnwrapper(_correlationMutatorMock.Object, _httpRequestUnwrapperMock.Object, _logger);
        }

        [Fact(DisplayName = "HttpCorrelationUnwrapper - UnwrapAsync - Read Correlation-Id header")]
        public async Task HttpCorrelationUnwrapper_UnwrapAsync_ReadCorrelationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var httpRequestMock = new Mock<HttpRequest>();
            var correlationId = Guid.NewGuid();

            httpRequestMock.SetupGet(hr => hr.Headers)
                .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
                {
                    { CorrelationDefaults.CorrelationId, correlationId.ToString() },
                    { CorrelationDefaults.CausationId, Guid.NewGuid().ToString() }
                }));

            _httpRequestUnwrapperMock.Setup(hru => hru.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Command(Guid.NewGuid()));

            // Act
            var request = await sut.UnwrapAsync(httpRequestMock.Object, CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(m => m.CorrelationId = correlationId);
        }

        [Fact(DisplayName = "HttpCorrelationUnwrapper - UnwrapAsync - Missed Correlation-Id header")]
        public async Task HttpCorrelationUnwrapper_UnwrapAsync_MissedCorrelationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var httpRequestMock = new Mock<HttpRequest>();

            httpRequestMock.SetupGet(hr => hr.Headers)
                .Returns(new HeaderDictionary(new Dictionary<string, StringValues>()));

            // Act
            // Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(async () => await sut.UnwrapAsync(httpRequestMock.Object, CancellationToken.None));
        }

        [Fact(DisplayName = "HttpCorrelationUnwrapper - UnwrapAsync - Read Causation-Id header")]
        public async Task HttpCorrelationUnwrapper_UnwrapAsync_ReadCausationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var httpRequestMock = new Mock<HttpRequest>();
            var causationId = Guid.NewGuid();

            httpRequestMock.SetupGet(hr => hr.Headers)
                .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
                {
                    { CorrelationDefaults.CorrelationId, Guid.NewGuid().ToString() },
                    { CorrelationDefaults.CausationId, causationId.ToString() }
                }));

            _httpRequestUnwrapperMock.Setup(hru => hru.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Command(Guid.NewGuid()));

            // Act
            var request = await sut.UnwrapAsync(httpRequestMock.Object, CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(m => m.CausationId = causationId);
        }

        [Fact(DisplayName = "HttpCorrelationUnwrapper - UnwrapAsync - Null Causation-Id header")]
        public async Task HttpCorrelationUnwrapper_UnwrapAsync_NullCausatuionIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var httpRequestMock = new Mock<HttpRequest>();

            httpRequestMock.SetupGet(hr => hr.Headers)
                .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
                {
                    { CorrelationDefaults.CorrelationId, Guid.NewGuid().ToString() },
                }));

            _httpRequestUnwrapperMock.Setup(hru => hru.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Command(Guid.NewGuid()));

            // Act
            var request = await sut.UnwrapAsync(httpRequestMock.Object, CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(m => m.CausationId = null);
        }

        [Fact(DisplayName = "HttpCorrelationUnwrapper - UnwrapAsync - Call next unwrapper")]
        public async Task HttpCorrelationUnwrapper_UnwrapAsync_CallNextUnwrapper()
        {
            // Arrange
            var sut = CreateSUT();
            var httpRequestMock = new Mock<HttpRequest>();

            httpRequestMock.SetupGet(hr => hr.Headers)
                .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
                {
                    { CorrelationDefaults.CorrelationId, Guid.NewGuid().ToString() },
                    { CorrelationDefaults.CausationId, Guid.NewGuid().ToString() }
                }));

            _httpRequestUnwrapperMock.Setup(hru => hru.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Command(Guid.NewGuid()));

            // Act
            await sut.UnwrapAsync(httpRequestMock.Object, CancellationToken.None);

            // Assert
            _httpRequestUnwrapperMock.Verify(hru => hru.UnwrapAsync(It.Is<HttpRequest>(hr => hr.Equals(httpRequestMock.Object)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HttpCorrelationUnwrapper - UnwrapAsync - Call next unwrapper")]
        public async Task HttpCorrelationUnwrapper_UnwrapAsync_SetMessageId()
        {
            // Arrange
            var sut = CreateSUT();
            var httpRequestMock = new Mock<HttpRequest>();
            var requestId = Guid.NewGuid();

            httpRequestMock.SetupGet(hr => hr.Headers)
                .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
                {
                    { CorrelationDefaults.CorrelationId, Guid.NewGuid().ToString() },
                    { CorrelationDefaults.CausationId, Guid.NewGuid().ToString() }
                }));

            _httpRequestUnwrapperMock.Setup(hru => hru.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Command(requestId));

            // Act
            await sut.UnwrapAsync(httpRequestMock.Object, CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(cm => cm.MessageId = requestId);
        }
    }
}
