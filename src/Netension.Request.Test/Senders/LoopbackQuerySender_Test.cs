using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Messages;
using Netension.Request.Receivers;
using Netension.Request.Senders;
using Netension.Request.Wrappers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class LoopbackQuerySender_Test
    {
        private readonly ILogger<LoopbackQuerySender> _logger;
        private Mock<ILoopbackRequestReceiver> _loopbackRequestReceiverMock;
        private Mock<ILoopbackRequestWrapper> _wrapperMock;

        public LoopbackQuerySender_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<LoopbackQuerySender>();
        }

        private LoopbackQuerySender CreateSUT()
        {
            _wrapperMock = new Mock<ILoopbackRequestWrapper>();
            _loopbackRequestReceiverMock = new Mock<ILoopbackRequestReceiver>();

            return new LoopbackQuerySender(_wrapperMock.Object, _loopbackRequestReceiverMock.Object, _logger);
        }

        [Fact(DisplayName = "LoopbackQuerySender - SendAsync - Wrap message")]
        public async Task LoopbackQuerySender_SendAsync_WrapMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();

            // Act
            await sut.QueryAsync(query, CancellationToken.None);

            // Assert
            _wrapperMock.Verify(w => w.WrapAsync(It.Is<Query<object>>(q => q.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "LoopbackCommandSender - SendAsync - Call receiver")]
        public async Task LoopbackCommandSender_SendAsync_CallReceiver()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();
            var message = new LoopbackMessage(query);

            _wrapperMock.Setup(w => w.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(message);

            // Act
            await sut.QueryAsync(query, CancellationToken.None);

            // Assert
            _loopbackRequestReceiverMock.Verify(lrr => lrr.ReceiveAsync(It.Is<LoopbackMessage>(lm => lm.Request.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
