using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Netension.Request.Abstraction.Requests;
using Netension.Request.NetCore.Asp.Options;
using Netension.Request.NetCore.Asp.Senders;
using Netension.Request.NetCore.Asp.Wrappers;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class HttpCommandSender_Test
    {
        private readonly Uri BASE_ADRESS = new Uri("http://test-uri");
        private readonly PathString PATH = "/test-path";

        private readonly ILogger<HttpCommandSender> _logger;
        private Mock<IHttpRequestWrapper> _wrapperMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<IOptions<HttpCommandSenderOptions>> _optionsMock;

        public HttpCommandSender_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<HttpCommandSender>();
        }

        private HttpCommandSender CreateSUT()
        {
            _wrapperMock = new Mock<IHttpRequestWrapper>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _optionsMock = new Mock<IOptions<HttpCommandSenderOptions>>();

            _optionsMock.SetupGet(o => o.Value).Returns(new HttpCommandSenderOptions { Path = PATH });

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage());

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = BASE_ADRESS
            };
            return new HttpCommandSender(httpClient, _optionsMock.Object, _wrapperMock.Object, _logger);
        }

        [Fact(DisplayName = "HttpCommandSender - SendAsync - Wrap message")]
        public async Task HttpCommandSender_SendAsync_WrapMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            // Act
            await sut.SendAsync(command, CancellationToken.None);

            // Assert
            _wrapperMock.Verify(w => w.WrapAsync(It.Is<IRequest>(r => r.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HttpCommandSender - SendAsync - Post content")]
        public async Task HttpCommandSender_SendAsync_PostContent()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            _wrapperMock.Setup(w => w.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonContent.Create(command));

            // Act
            await sut.SendAsync(new Command(), CancellationToken.None);

            // Assert
            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(hrm => hrm.Verify(command)), ItExpr.IsAny<CancellationToken>());
        }
    }

    public static class HttpCommandSenderExtensions
    {
        public static bool Verify(this HttpRequestMessage requestMessage, Command command)
        {
            return command.Equals(requestMessage.Content.ReadFromJsonAsync<Command>().Result);
        }
    }
}
