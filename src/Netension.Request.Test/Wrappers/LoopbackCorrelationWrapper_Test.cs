using Microsoft.Extensions.Logging;
using Moq;
using Netension.Extensions.Correlation;
using Netension.Request.Wrappers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Netension.Request.Messages;
using Netension.Request.Abstraction.Requests;

namespace Netension.Request.Test.Wrappers
{
    public class LoopbackCorrelationWrapper_Test
    {
        private readonly ILogger<LoopbackCorrelationWrapper> _logger;
        private Mock<ICorrelationAccessor> _correlationAccessorMock;
        private Mock<ILoopbackRequestWrapper> _loopbackRequestWrapperMock;

        public LoopbackCorrelationWrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<LoopbackCorrelationWrapper>();
        }

        private LoopbackCorrelationWrapper CreateSUT()
        {
            _correlationAccessorMock = new Mock<ICorrelationAccessor>();
            _loopbackRequestWrapperMock = new Mock<ILoopbackRequestWrapper>();

            return new LoopbackCorrelationWrapper(_correlationAccessorMock.Object, _loopbackRequestWrapperMock.Object, _logger);
        }

        [Fact(DisplayName = "LoopbackCorrelationWrapper - WrapAsync - Set correlation header")]
        public async Task LoopbackCorrelationWrapper_WrapAsync_SetCorrelationHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var correlationId = Guid.NewGuid();

            _loopbackRequestWrapperMock.Setup(lrw => lrw.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LoopbackMessage(new Command()));
            _correlationAccessorMock.SetupGet(ca => ca.CorrelationId).Returns(correlationId);

            // Act
            var message = await sut.WrapAsync(new Command(), CancellationToken.None);

            // Assert
            Assert.Equal(correlationId, message.Headers.GetCorrelationId());
        }

        [Fact(DisplayName = "LoopbackCorrelationWrapper - WrapAsync - Generate correlation id")]
        public async Task LoopbackCorrelationWrapper_WrapAsync_GenerateCorrelationId()
        {
            // Arrange
            var sut = CreateSUT();

            _loopbackRequestWrapperMock.Setup(lrw => lrw.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LoopbackMessage(new Command()));

            // Act
            var message = await sut.WrapAsync(new Command(), CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, message.Headers.GetCorrelationId());
        }

        [Fact(DisplayName = "LoopbackCorrelationWrapper - WrapAsync - Set causation header")]
        public async Task LoopbackCorrelationWrapper_WrapAsync_SetCausationHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var requestId = Guid.NewGuid();

            _loopbackRequestWrapperMock.Setup(lrw => lrw.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LoopbackMessage(new Command(requestId)));

            // Act
            var message = await sut.WrapAsync(new Command(requestId), CancellationToken.None);

            // Assert
            Assert.Equal(requestId, message.Headers.GetCausationId());
        }
    }
}
