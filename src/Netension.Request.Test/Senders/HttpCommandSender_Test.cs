using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Moq.Protected;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Requests;
using Netension.Request.NetCore.Asp.Enumerations;
using Netension.Request.NetCore.Asp.Middlewares;
using Netension.Request.NetCore.Asp.Options;
using Netension.Request.NetCore.Asp.Senders;
using Netension.Request.NetCore.Asp.ValueObjects;
using Netension.Request.NetCore.Asp.Wrappers;
using System;
using System.Linq;
using System.Net;
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
        private HttpSenderOptions _options;

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
            _options = new HttpSenderOptions { Path = PATH };

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage());

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = BASE_ADRESS
            };
            return new HttpCommandSender(httpClient, _options, _wrapperMock.Object, _logger);
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

        [Fact(DisplayName = "HttpCommandSender - SendAsync - Request null")]
        public async Task HttpCommandSender_SendAsync_RequestNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.SendAsync<ICommand>(null, CancellationToken.None));
        }

        [Fact(DisplayName = "HttpCommandSender - SendAsync - VerificationException")]
        public async Task HttpCommandSender_SendAsync_VerificationException()
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
            await ExceptionAssert.ThrowsAsync<VerificationException>(async () => await sut.SendAsync(new Command(), CancellationToken.None), exception =>
            {
                Assert.Equal(errorCode, exception.Code);
                Assert.Equal(message, exception.Message);
            });
        }

        [Fact(DisplayName = "HttpCommandSender - SendAsync - ValidationException")]
        public async Task HttpCommandSender_SendAsync_ValidationException()
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
            await ExceptionAssert.ThrowsAsync<ValidationException>(async () => await sut.SendAsync(new Command(), CancellationToken.None), exception =>
            {
                Assert.Collection(exception.Errors, e => failures.Any(f => f.Equals(e)), e => failures.Any(f => f.Equals(e)), e => failures.Any(f => f.Equals(e)));
            });
        }

        [Fact(DisplayName = "HttpCommandSender - SendAsync - Exception")]
        public async Task HttpCommandSender_SendAsync_Exception()
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
            await Assert.ThrowsAnyAsync<Exception>(async () => await sut.SendAsync(new Command(), CancellationToken.None));
        }
    }

    public static class HttpCommandSenderExtensions
    {
        public static bool Verify(this HttpRequestMessage requestMessage, Command command)
        {
            return command.Equals(requestMessage.Content.ReadFromJsonAsync<Command>().Result);
        }
    }

    public static class ExceptionAssert
    {
        public static async Task ThrowsAsync<TException>(Func<Task> testCode, Action<TException> validate)
            where TException : Exception
        {
            validate(await Assert.ThrowsAsync<TException>(testCode));
        }
    }
}