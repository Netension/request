using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.NetCore.Asp.Unwrappers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Wrappers
{
    public class HttpRequestUnwrapper_Test
    {
        private readonly ILogger<HttpRequestUnwrapper> _logger;

        public HttpRequestUnwrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<HttpRequestUnwrapper>();
        }

        private HttpRequestUnwrapper CreateSUT()
        {
            return new HttpRequestUnwrapper(_logger);
        }

        [Fact(DisplayName = "HttpRequestUnwrapper - UnwrapAsync - Parse message")]
        public async Task HttpRequestUnwrapper_UnwrapAsync_ParseMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            var bodyMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command)));

            httpRequestMock.SetupGet(hr => hr.Headers).Returns(new HeaderDictionary(new Dictionary<string, StringValues>
            {
                { RequestDefaults.Header.MessageType, command.MessageType }
            }));

            httpContextMock.SetupGet(hc => hc.Request).Returns(httpRequestMock.Object);

            httpRequestMock.SetupGet(hr => hr.ContentType).Returns("application/json");
            httpRequestMock.SetupGet(hr => hr.HttpContext).Returns(httpContextMock.Object);
            httpRequestMock.SetupGet(hr => hr.Body).Returns(bodyMemoryStream);
            httpRequestMock.SetupGet(hr => hr.BodyReader).Returns(PipeReader.Create(bodyMemoryStream));

            // Act
            var request = await sut.UnwrapAsync(httpRequestMock.Object, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.Equal(command, request);
        }

        [Fact(DisplayName = "HttpRequestUnwrapper - UnwrapAsync - Message-Type header missing")]
        public async Task HttpRequestUnwrapper_UnwrapAsync_MessageTypeHeaderMissing()
        {
            // Arrange
            var sut = CreateSUT();
            var httpRequestMock = new Mock<HttpRequest>();

            httpRequestMock.SetupGet(hr => hr.Headers).Returns(new HeaderDictionary());

            // Act
            // Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(async () => await sut.UnwrapAsync(httpRequestMock.Object, CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }
}
