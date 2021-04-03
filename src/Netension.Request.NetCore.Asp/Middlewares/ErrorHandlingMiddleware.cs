using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Request.NetCore.Asp.Enumerations;
using System;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (VerificationException exception)
            {
                _logger.LogError(exception, ErrorCodeEnumeration.VerificationError.Message);
                await context.Response.WriteErrorAsync(StatusCodes.Status400BadRequest, exception, context.RequestAborted);

            }
            catch (ValidationException exception)
            {
                _logger.LogError(exception, ErrorCodeEnumeration.ValidationFailed.Message);
                await context.Response.WriteErrorAsync(StatusCodes.Status400BadRequest, exception, context.RequestAborted);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, ErrorCodeEnumeration.InternalServerError.Message);
                await context.Response.WriteErrorAsync(StatusCodes.Status500InternalServerError, exception, context.RequestAborted);
            }
        }
    }

    public class Error
    {
        public int Code { get; }
        public string Message { get; }

        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public static class ErrorHandlingExtensions
    {
        public static async Task WriteErrorAsync<TException>(this HttpResponse response, int statusCode, TException exception, CancellationToken cancellationToken)
            where TException : Exception
        {
            response.StatusCode = statusCode;
            response.ContentType = MediaTypeNames.Application.Json;
            await response.Body.WriteAsync(exception.Encode(), cancellationToken);
        }

        public static ReadOnlyMemory<byte> Encode(this ValidationException exception)
        {
            var message = new StringBuilder(ErrorCodeEnumeration.ValidationFailed.Message);
            message.AppendLine();

            foreach (var failure in exception.Errors)
            {
                message.AppendLine($"--{failure.PropertyName}: {failure.ErrorMessage}");
            }

            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(message));
        }

        public static ReadOnlyMemory<byte> Encode(this Exception exception)
        {
            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(new Error(ErrorCodeEnumeration.InternalServerError.Id, ErrorCodeEnumeration.InternalServerError.Message)));
        }

        public static ReadOnlyMemory<byte> Serialize(this VerificationException exception)
        {
            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(new Error(exception.Code, exception.Message)));
        }
    }

}
