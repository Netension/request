using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Extensions;
using Netension.Request.Messages;
using Netension.Request.Receivers;
using Netension.Request.Unwrappers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Receivers
{
    public class LoopbackRequestReceiver_Test
    {
        private readonly ILogger<LoopbackRequestReceiver> _logger;
        private Mock<ILoopbackRequestUnwrapper> _unwrapperMock;
        private Mock<ICommandDispatcher> _commandDispatcherMock;
        private Mock<IQueryDispatcher> _queryDispatcherMock;

        public LoopbackRequestReceiver_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<LoopbackRequestReceiver>();
        }

        private LoopbackRequestReceiver CreateSUT()
        {
            _unwrapperMock = new Mock<ILoopbackRequestUnwrapper>();
            _commandDispatcherMock = new Mock<ICommandDispatcher>();
            _queryDispatcherMock = new Mock<IQueryDispatcher>();

            return new LoopbackRequestReceiver(_commandDispatcherMock.Object, _queryDispatcherMock.Object, _unwrapperMock.Object, _logger);
        }

        [Fact(DisplayName = "LoopbackRequestReceiver - ReceiveAsync - Unwrap message")]
        public async Task LoopbackRequestReceiver_ReceiveAsync_UnwrapMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new LoopbackMessage(new Query<object>());
            message.Headers.SetMessageType(message.Request.MessageType);

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<LoopbackMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(message.Request);

            // Act
            await sut.ReceiveAsync(message, CancellationToken.None);

            // Assert
            _unwrapperMock.Verify(u => u.UnwrapAsync(It.Is<LoopbackMessage>(lm => lm.Equals(message)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "LoopbackRequestReceiver - ReceiveAsync - Dispatch command")]
        public async Task LoopbackRequestReceiver_ReceiveAsync_DispatchCommand()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new LoopbackMessage(new Command());
            message.Headers.SetMessageType(message.Request.MessageType);

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<LoopbackMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(message.Request);

            // Act
            await sut.ReceiveAsync(message, CancellationToken.None);

            // Assert
            _commandDispatcherMock.Verify(c => c.DispatchAsync(It.Is<ICommand>(c => c.Equals(message.Request)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "LoopbackRequestReceiver - ReceiveAsync - Dispatch query")]
        public async Task LoopbackRequestReceiver_ReceiveAsync_DispatchQuery()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new LoopbackMessage(new Query<object>());
            message.Headers.SetMessageType(message.Request.MessageType);

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<LoopbackMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(message.Request);

            // Act
            await sut.ReceiveAsync(message, CancellationToken.None);

            // Assert
            _queryDispatcherMock.Verify(c => c.DispatchAsync(It.Is<Query<object>>(c => c.Equals(message.Request)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
