using Microsoft.Extensions.Logging;
using Moq;
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

            return new LoopbackQuerySender(_wrapperMock.Object, _logger);
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
    }
}
