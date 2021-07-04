using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Messages;
using Netension.Request.Receivers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Receivers
{
    public class LoopbackScopeHandler_Test
    {
        private readonly ILogger<LoopbackScopeHandler> _logger;
        private Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private Mock<ILoopbackRequestReceiver> _loopbackRequestReceiverMock;

        public LoopbackScopeHandler_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<LoopbackScopeHandler>();
        }

        private LoopbackScopeHandler CreateSUT()
        {
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _loopbackRequestReceiverMock = new Mock<ILoopbackRequestReceiver>();

            return new LoopbackScopeHandler(_serviceScopeFactoryMock.Object, _loopbackRequestReceiverMock.Object, _logger);
        }

        [Fact(DisplayName = "LoopbackScopeHandler - HandleAsync - Create scope")]
        public async Task LoopbackScopeHandler_HandleAsync_CreateScope()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.ReceiveAsync(new LoopbackMessage(new Command()), CancellationToken.None).ConfigureAwait(false);

            // Assert
            _serviceScopeFactoryMock.Verify(ssf => ssf.CreateScope(), Times.Once);
        }

        [Fact(DisplayName = "LoopbackScopeHandler - HandleAsync - Call next handle receiver")]
        public async Task LoopbackScopeHandler_HandleAsync_CallNextReceiver()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new LoopbackMessage(new Command());

            // Act
            await sut.ReceiveAsync(message, CancellationToken.None).ConfigureAwait(false);

            // Assert
            _loopbackRequestReceiverMock.Verify(lrr => lrr.ReceiveAsync(It.Is<LoopbackMessage>(lm => lm.Equals(message)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
