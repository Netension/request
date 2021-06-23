using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Core.Exceptions;
using Netension.Request.NetCore.Asp.Middlewares;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Request.Test.Middlewares
{
    public class ErrorHandlingMiddleware_Test
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private Mock<HttpContext> _httpContextMock;

        public ErrorHandlingMiddleware_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<ErrorHandlingMiddleware>();
        }


        private ErrorHandlingMiddleware CreateSUT(RequestDelegate next)
        {
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.SetupGet(hc => hc.RequestAborted)
                .Returns(new CancellationToken());

            return new ErrorHandlingMiddleware(next, _logger);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - ValidationException - SetStatusCodeTo400")]
        public async Task ErrorHandlingMiddleware_ValidationException_SetStatusCodeTo400()
        {
            // Arrange
            var sut = CreateSUT(context => throw new ValidationException(new Fixture().CreateMany<ValidationFailure>(2)));
            var httpResponseMock = new Mock<HttpResponse>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(new Mock<Stream>().Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            httpResponseMock.VerifySet(hr => hr.StatusCode = StatusCodes.Status400BadRequest);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - ValidationException - SetContentTypeToJson")]
        public async Task ErrorHandlingMiddleware_ValidationException_SetContentTypeToJson()
        {
            // Arrange
            var sut = CreateSUT(context => throw new ValidationException(new Fixture().CreateMany<ValidationFailure>(2)));
            var httpResponseMock = new Mock<HttpResponse>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(new Mock<Stream>().Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            httpResponseMock.VerifySet(hr => hr.ContentType = MediaTypeNames.Application.Json);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - ValidationException - SetContent")]
        public async Task ErrorHandlingMiddleware_ValidationException_SetContent()
        {
            // Arrange
            var exception = new ValidationException(new Fixture().CreateMany<ValidationFailure>(2));
            var sut = CreateSUT(context => throw exception);
            var httpResponseMock = new Mock<HttpResponse>();
            var bodyMock = new Mock<Stream>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(bodyMock.Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            bodyMock.Verify(b => b.WriteAsync(It.Is<ReadOnlyMemory<byte>>(c => c.Validate(exception)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - VerificationException - SetStatusCodeTo400")]
        public async Task ErrorHandlingMiddleware_VerificationException_SetStatusCodeTo400()
        {
            // Arrange
            var sut = CreateSUT(context => throw new Fixture().Create<VerificationException>());
            var httpResponseMock = new Mock<HttpResponse>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(new Mock<Stream>().Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            httpResponseMock.VerifySet(hr => hr.StatusCode = StatusCodes.Status400BadRequest);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - VerificationException - SetContentTypeToJson", Skip = "")]
        public async Task ErrorHandlingMiddleware_VerificationException_SetContentTypeToJson()
        {
            // Arrange
            var sut = CreateSUT(context => throw new Fixture().Create<VerificationException>());
            var httpResponseMock = new Mock<HttpResponse>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(new Mock<Stream>().Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            httpResponseMock.VerifySet(hr => hr.ContentType = MediaTypeNames.Application.Json);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - VerificationException - SetContent")]
        public async Task ErrorHandlingMiddleware_VerificationException_SetContent()
        {
            // Arrange
            var exception = new Fixture().Create<VerificationException>();
            var sut = CreateSUT(context => throw exception);
            var httpResponseMock = new Mock<HttpResponse>();
            var bodyMock = new Mock<Stream>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(bodyMock.Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            bodyMock.Verify(b => b.WriteAsync(It.Is<ReadOnlyMemory<byte>>(c => c.Validate(exception)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - Exception - SetStatusCodeTo500")]
        public async Task ErrorHandlingMiddleware_Exception_SetStatusCodeTo500()
        {
            // Arrange
            var sut = CreateSUT(context => throw new Fixture().Create<Exception>());
            var httpResponseMock = new Mock<HttpResponse>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(new Mock<Stream>().Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            httpResponseMock.VerifySet(hr => hr.StatusCode = StatusCodes.Status500InternalServerError);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - Exception - SetContentTypeToJson")]
        public async Task ErrorHandlingMiddleware_Exception_SetContentTypeToJson()
        {
            // Arrange
            var sut = CreateSUT(context => throw new Fixture().Create<Exception>());
            var httpResponseMock = new Mock<HttpResponse>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(new Mock<Stream>().Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            httpResponseMock.VerifySet(hr => hr.ContentType = MediaTypeNames.Application.Json);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - Exception - SetContent")]
        public async Task ErrorHandlingMiddleware_Exception_SetContent()
        {
            // Arrange
            var exception = new Fixture().Create<Exception>();
            var sut = CreateSUT(context => throw exception);
            var httpResponseMock = new Mock<HttpResponse>();
            var bodyMock = new Mock<Stream>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(bodyMock.Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            bodyMock.Verify(b => b.WriteAsync(It.Is<ReadOnlyMemory<byte>>(c => c.Validate(exception)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "ErrorHandlingMiddleware - NotFoundException - HttpCode")]
        public async Task ErrorHandlingMiddleware_NotFoundException_HttpCode()
        {
            // Arrange
            var exception = new Fixture().Create<NotFoundException>();
            var sut = CreateSUT(context => throw exception);
            var httpResponseMock = new Mock<HttpResponse>();
            var bodyMock = new Mock<Stream>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(bodyMock.Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            httpResponseMock.VerifySet(hr => hr.StatusCode = StatusCodes.Status404NotFound);
        }


        [Fact(DisplayName = "ErrorHandlingMiddleware - ConflictException - HttpCode")]
        public async Task ErrorHandlingMiddleware_ConflictException_HttpCode()
        {
            // Arrange
            var exception = new Fixture().Create<ConflictException>();
            var sut = CreateSUT(context => throw exception);
            var httpResponseMock = new Mock<HttpResponse>();
            var bodyMock = new Mock<Stream>();

            httpResponseMock.SetupGet(hr => hr.Body)
                .Returns(bodyMock.Object);

            _httpContextMock.SetupGet(hc => hc.Response)
                .Returns(httpResponseMock.Object);

            // Act
            await sut.InvokeAsync(_httpContextMock.Object);

            // Assert
            httpResponseMock.VerifySet(hr => hr.StatusCode = StatusCodes.Status409Conflict);
        }
    }

    public static class TestExtensions
    {
        public static bool Validate(this ReadOnlyMemory<byte> content, ValidationException exception)
        {
            var expected = exception.GetBytes();
            return expected.ToArray().SequenceEqual(content.ToArray());
        }

        public static bool Validate(this ReadOnlyMemory<byte> content, VerificationException exception)
        {
            var expected = exception.GetBytes();
            return expected.ToArray().SequenceEqual(content.ToArray());
        }

        public static bool Validate(this ReadOnlyMemory<byte> content, Exception exception)
        {
            var expected = exception.GetBytes();
            return expected.ToArray().SequenceEqual(content.ToArray());
        }
    }
}
