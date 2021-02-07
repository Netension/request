using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Senders;
using Netension.Request.Wrappers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class LoopbackCommandSender_Test
    {
        private Mock<ILoopbackRequestWrapper> _wrapperMock;
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

            return new LoopbackCommandSender(_wrapperMock.Object, _logger);
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
    }
}
