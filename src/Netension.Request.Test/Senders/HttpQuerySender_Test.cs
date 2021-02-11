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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class HttpQuerySender_Test
    {
        private readonly Uri BASE_ADRESS = new Uri("http://test-uri");
        private readonly PathString PATH = "/test-path";

        private readonly ILogger<HttpQuerySender> _logger;
        private Mock<IHttpRequestWrapper> _wrapperMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<IOptions<HttpSenderOptions>> _optionsMock;
        private Mock<IOptions<JsonSerializerOptions>> _jsonSerializerOptionsMock;

        public HttpQuerySender_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<HttpQuerySender>();
        }

        private HttpQuerySender CreateSUT()
        {
            _wrapperMock = new Mock<IHttpRequestWrapper>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _optionsMock = new Mock<IOptions<HttpSenderOptions>>();
            _jsonSerializerOptionsMock = new Mock<IOptions<JsonSerializerOptions>>();

            _optionsMock.SetupGet(o => o.Value).Returns(new HttpSenderOptions { Path = PATH });

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent("{}")
                });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = BASE_ADRESS
            };

            return new HttpQuerySender(httpClient, _jsonSerializerOptionsMock.Object, _optionsMock.Object, _wrapperMock.Object, _logger);
        }

        [Fact(DisplayName = "HttpQuerySender - QueryAsync - Wrap message")]
        public async Task HttpQuerySender_QueryAsync_WrapMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();

            // Act
            await sut.QueryAsync(query, CancellationToken.None);

            // Assert
            _wrapperMock.Verify(w => w.WrapAsync(It.Is<IRequest>(r => r.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HttpQuerySender - QueryAsync - Post content")]
        public async Task HttpCommandSender_SendAsync_PostContent()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();

            _wrapperMock.Setup(w => w.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonContent.Create(query));

            // Act
            await sut.QueryAsync(query, CancellationToken.None);

            // Assert
            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(hrm => hrm.Verify(query)), ItExpr.IsAny<CancellationToken>());
        }
    }

    public static class HttpQuerySenderExtensions
    {
        public static bool Verify(this HttpRequestMessage requestMessage, Query<object> query)
        {
            return query.Equals(requestMessage.Content.ReadFromJsonAsync<Query<object>>().Result);
        }
    }
}
