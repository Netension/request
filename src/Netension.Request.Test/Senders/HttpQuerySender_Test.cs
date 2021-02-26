using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
    public class HttpQuerySender_Test
    {
        private readonly Uri BASE_ADRESS = new Uri("http://test-uri");
        private readonly PathString PATH = "/test-path";

        private readonly ILogger<HttpQuerySender> _logger;
        private Mock<IHttpRequestWrapper> _wrapperMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpSenderOptions _options;

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
            _options = new HttpSenderOptions { Path = PATH };

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent("{}")
                });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = BASE_ADRESS
            };

            return new HttpQuerySender(httpClient, _options, _wrapperMock.Object, _logger);
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
        public async Task HttpQuerySender_SendAsync_PostContent()
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

        [Fact(DisplayName = "HttpQuerySender - QueryAsync - Request null")]
        public async Task HttpQuerySender_QueryAsync_RequestNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.QueryAsync<IQuery<object>>(null, CancellationToken.None));
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
