using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Http.Enumerations;
using Netension.Request.Http.Options;
using Netension.Request.Http.Senders;
using Netension.Request.Http.ValueObjects;
using Netension.Request.Http.Wrappers;
using Netension.Request.NetCore.Asp.Middlewares;
using Netension.Request.Test.Extensions;
using System;
using System.Net;
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
        private readonly Uri BASE_ADRESS = new("http://test-uri");
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

        [Fact(DisplayName = "[HQS001]: Wrap message")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_QueryAsync_WrapMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();

            // Act
            await sut.QueryAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            _wrapperMock.Verify(w => w.WrapAsync(It.Is<IRequest>(r => r.Equals(query)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "[HQS002]: Post content")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_SendAsync_PostContent()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Query<object>();

            _wrapperMock.Setup(w => w.WrapAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonContent.Create(query));

            // Act
            await sut.QueryAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(hrm => hrm.Verify(query)), ItExpr.IsAny<CancellationToken>());
        }

        [Fact(DisplayName = "[HQS003]: Null request")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_QueryAsync_RequestNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.QueryAsync<IQuery<object>>(null, CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "[HQS004]: Verification exception")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_QueryAsync_VerificationException()
        {
            // Arrange
            var sut = CreateSUT();
            var errorCode = new Fixture().Create<int>();
            var message = new Fixture().Create<string>();

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = JsonContent.Create(new Error(errorCode, message))
                });

            // Act
            //Assert
            await ExceptionAssert.ThrowsAsync<VerificationException>(async () => await sut.QueryAsync(new Query<object>(), CancellationToken.None).ConfigureAwait(false), exception =>
            {
                Assert.Equal(errorCode, exception.Code);
                Assert.Equal(message, exception.Message);
            }).ConfigureAwait(false);
        }

        [Fact(DisplayName = "[HQS005]: Validation exception")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_QueryAsync_ValidationException()
        {
            // Arrange
            var sut = CreateSUT();
            var failures = new Fixture()
                            .Build<ValidationFailure>()
                                .OmitAutoProperties()
                                .With(vf => vf.PropertyName, new Fixture().Create<string>())
                                .With(vf => vf.ErrorMessage, new Fixture().Create<string>())
                            .CreateMany(3);

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = JsonContent.Create(new ValidationException(failures).ToError())
                });

            // Act
            //Assert
            await ExceptionAssert.ThrowsAsync<ValidationException>(async () => await sut.QueryAsync(new Query<object>(), CancellationToken.None).ConfigureAwait(false), exception => Assert.Collection(exception.Errors, failures.Validate, failures.Validate, failures.Validate)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "[HQS006]: Internal server error")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_SendAsync_Exception()
        {
            // Arrange
            var sut = CreateSUT();

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = JsonContent.Create(new Error(ErrorCodeEnumeration.InternalServerError.Id, ErrorCodeEnumeration.InternalServerError.Message))
                });

            // Act
            //Assert
            await Assert.ThrowsAnyAsync<Exception>(async () => await sut.QueryAsync(new Query<object>(), CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "[HQS007]: Resource not found")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_SendAsync_NotFound()
        {
            // Arrange
            var sut = CreateSUT();

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            // Act
            //Assert
            var exception = await Assert.ThrowsAnyAsync<VerificationException>(async () => await sut.QueryAsync(new Query<object>(), CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.Equal(ErrorCodeEnumeration.NotFound.Id, exception.Code);
            Assert.Equal(ErrorCodeEnumeration.NotFound.Message, exception.Message);
        }

        [Fact(DisplayName = "[HQS008]: Unathorized")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_SendAsync_Unathorized()
        {
            // Arrange
            var sut = CreateSUT();

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized });

            // Act
            //Assert
            var exception = await Assert.ThrowsAnyAsync<VerificationException>(async () => await sut.QueryAsync(new Query<object>(), CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.Equal(ErrorCodeEnumeration.Unathorized.Id, exception.Code);
            Assert.Equal(ErrorCodeEnumeration.Unathorized.Message, exception.Message);
        }

        [Fact(DisplayName = "[HQS009]: Forbidden")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_SendAsync_Forbidden()
        {
            // Arrange
            var sut = CreateSUT();

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Forbidden });

            // Act
            //Assert
            var exception = await Assert.ThrowsAnyAsync<VerificationException>(async () => await sut.QueryAsync(new Query<object>(), CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.Equal(ErrorCodeEnumeration.Forbidden.Id, exception.Code);
            Assert.Equal(ErrorCodeEnumeration.Forbidden.Message, exception.Message);
        }

        [Fact(DisplayName = "[HQS010]: Conflict")]
        [Trait("Feature", "SQ: Send Query")]
        public async Task HttpQuerySender_SendAsync_Conflict()
        {
            // Arrange
            var sut = CreateSUT();

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Conflict });

            // Act
            //Assert
            var exception = await Assert.ThrowsAnyAsync<VerificationException>(async () => await sut.QueryAsync(new Query<object>(), CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.Equal(ErrorCodeEnumeration.Conflict.Id, exception.Code);
            Assert.Equal(ErrorCodeEnumeration.Conflict.Message, exception.Message);
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
