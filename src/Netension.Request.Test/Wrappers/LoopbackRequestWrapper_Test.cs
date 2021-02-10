using Netension.Request.Wrappers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Netension.Request.Extensions;

namespace Netension.Request.Test.Wrappers
{
    public class LoopbackRequestWrapper_Test
    {
        private readonly ILogger<LoopbackRequestWrapper> _logger;

        public LoopbackRequestWrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<LoopbackRequestWrapper>();
        }

        private LoopbackRequestWrapper CreateSUT()
        {
            return new LoopbackRequestWrapper(_logger);
        }

        [Fact(DisplayName = "LoopbackRequestWrapper - Set Request")]
        public async Task LoopbackRequestWrapper_SetRequest()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            // Act
            var message = await sut.WrapAsync(command, CancellationToken.None);

            // Assert
            Assert.Equal(command, message.Request);
        }

        [Fact(DisplayName = "LoopbackRequestWrapper - Set type header")]
        public async Task LoopbackRequestWrapper_SetTypeHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            // Act
            var message = await sut.WrapAsync(command, CancellationToken.None);

            // Assert
            Assert.Equal(command.MessageType, message.Headers.GetMessageType());
        }
    }
}
