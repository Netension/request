using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Netension.Request.Http.Wrappers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Wrappers
{
    public class HttpRequestWrapper_Test
    {
        private readonly ILogger<HttpRequestWrapper> _logger;
        private Mock<IOptions<JsonSerializerOptions>> _optionsMock;

        public HttpRequestWrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<HttpRequestWrapper>();
        }

        private HttpRequestWrapper CreateSUT()
        {
            _optionsMock = new Mock<IOptions<JsonSerializerOptions>>();
            _optionsMock.Setup(o => o.Value)
                .Returns(new JsonSerializerOptions());

            return new HttpRequestWrapper(_optionsMock.Object, _logger);
        }

        [Fact(DisplayName = "HttpRequestWrapper - WrapAsync - Set Message-Type header")]
        public async Task HttpRequestWrapper_WrapAsync_SetMessageTypeHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            // Act
            var content = await sut.WrapAsync(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.Equal(command.MessageType, content.Headers.GetMessageType());
        }

        [Fact(DisplayName = "HttpRequestWrapper - WrapAsync - Set content")]
        public async Task HttpRequestWrapper_WrapAsync_SetContent()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            // Act
            var content = await sut.WrapAsync(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.Equal(command, await content.ReadFromJsonAsync<Command>().ConfigureAwait(false));
        }
    }
}
