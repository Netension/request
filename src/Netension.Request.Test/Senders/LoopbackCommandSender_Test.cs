using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Messages;
using Netension.Request.Receivers;
using Netension.Request.Senders;
using Netension.Request.Wrappers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class LoopbackCommandSender_Test
    {
        private Mock<ILoopbackRequestWrapper> _wrapperMock;
        private Mock<ILoopbackRequestReceiver> _loopbackRequestReceiverMock;
        private readonly ILogger<LoopbackCommandSender> _logger;

        public LoopbackCommandSender_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<LoopbackCommandSender>();
        }

        private LoopbackCommandSender CreateSUT()
        {
            _wrapperMock = new Mock<ILoopbackRequestWrapper>();
            _loopbackRequestReceiverMock = new Mock<ILoopbackRequestReceiver>();

            return new LoopbackCommandSender(_wrapperMock.Object, _loopbackRequestReceiverMock.Object, _logger);
        }

        [Fact(DisplayName = "LoopbackCommandSender - SendAsync - Wrap message")]
        public async Task LoopbackCommandSender_SendAsync_WrapMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            // Act
            await sut.SendAsync(command, CancellationToken.None);

            // Assert
            _wrapperMock.Verify(w => w.WrapAsync(It.Is<IRequest>(r => r.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "LoopbackCommandSender - SendAsync - Call receiver")]
        public async Task LoopbackCommandSender_SendAsync_CallReceiver()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();
            var message = new LoopbackMessage(command);

            _wrapperMock.Setup(w => w.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(message);

            // Act
            await sut.SendAsync(command, CancellationToken.None);

            // Assert
            _loopbackRequestReceiverMock.Verify(lrr => lrr.ReceiveAsync(It.Is<LoopbackMessage>(lm => lm.Request.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "LoopbackCommandSender - SendAsync - Null request")]
        public async Task LoopbackCommandSender_SendAsync_NullRequest()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.SendAsync<Command>(null, CancellationToken.None));
        }
    }
}
