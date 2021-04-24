using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.NetCore.Asp.Receivers;
using Netension.Request.NetCore.Asp.Unwrappers;
using Netension.Request.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Receivers
{
    public class TestRequest : BaseRequest
    {
        public TestRequest()
            : base(Guid.NewGuid())
        {
        }
    }

    public class HttpRequestReceiver_Test
    {
        private readonly ILogger<HttpRequestReceiver> _logger;
        private Mock<IHttpRequestUnwrapper> _unwrapperMock;
        private Mock<ICommandDispatcher> _commandDispatcherMock;
        private Mock<IQueryDispatcher> _queryDispatcherMock;

        public HttpRequestReceiver_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<HttpRequestReceiver>();
        }

        private HttpRequestReceiver CreateSUT()
        {
            _unwrapperMock = new Mock<IHttpRequestUnwrapper>();
            _commandDispatcherMock = new Mock<ICommandDispatcher>();
            _queryDispatcherMock = new Mock<IQueryDispatcher>();

            return new HttpRequestReceiver(_unwrapperMock.Object, _commandDispatcherMock.Object, _queryDispatcherMock.Object, _logger);
        }

        [Fact(DisplayName = "HttpRequestReceiver - ReceiveAsync - UnwrapMessage")]
        public async Task HttpRequestReceiver_ReceiveAsync_UnwrapMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var httpRequestMock = new Mock<HttpRequest>();

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Command());

            // Act
            await sut.ReceiveAsync(httpRequestMock.Object, CancellationToken.None);

            // Assert
            _unwrapperMock.Verify(uw => uw.UnwrapAsync(It.Is<HttpRequest>(hr => hr.Equals(httpRequestMock.Object)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HttpRequestReceiver - ReceiveAsync - Dispatch command")]
        public async Task HttpRequestReceiver_ReceiveAsync_DispatchCommand()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Command();

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(command);

            // Act
            await sut.ReceiveAsync(new Mock<HttpRequest>().Object, CancellationToken.None);

            // Assert
            _commandDispatcherMock.Verify(cd => cd.DispatchAsync(It.Is<ICommand>(c => c.Equals(command)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HttpRequestReceiver - ReceiveAsync - Dispatch query")]
        public async Task HttpRequestReceiver_ReceiveAsync_DispatchQuery()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(query);

            // Act
            await sut.ReceiveAsync(new Mock<HttpRequest>().Object, CancellationToken.None);

            // Assert
            _queryDispatcherMock.Verify(qd => qd.DispatchAsync<object>(It.IsAny<Query<object>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HttpRequestReceiver - ReceiveAsync - Unsupported Message-Type")]
        public async Task HttpRequestReceiver_ReceiveAsync_UnsupportedMessageType()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new TestRequest();

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<HttpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(request);

            // Act
            // Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(async () => await sut.ReceiveAsync(new Mock<HttpRequest>().Object, CancellationToken.None));
            ;
        }
    }
}
