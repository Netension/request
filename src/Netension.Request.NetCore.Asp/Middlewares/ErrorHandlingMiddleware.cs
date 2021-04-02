using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Request.NetCore.Asp.Enumerations;
using System;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
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
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(exception.Serialize()), context.RequestAborted);

            }
            catch (ValidationException exception)
            {
                _logger.LogError(exception, ErrorCodeEnumeration.ValidationFailed.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(exception.Serialize()), context.RequestAborted);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, ErrorCodeEnumeration.InternalServerError.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(exception.Serialize()), context.RequestAborted);
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

    public static class ExceptionExtensions
    {
        public static Error Serialize(this ValidationException exception)
        {
            var message = new StringBuilder(ErrorCodeEnumeration.ValidationFailed.Message);
            message.AppendLine();

            foreach (var failure in exception.Errors)
            {
                message.AppendLine($"--{failure.PropertyName}: {failure.ErrorMessage}");
            }

            return new Error(ErrorCodeEnumeration.ValidationFailed.Id, message.ToString());
        }

        public static Error Serialize(this Exception exception)
        {
            return new Error(ErrorCodeEnumeration.InternalServerError.Id, ErrorCodeEnumeration.InternalServerError.Message);
        }

        public static Error Serialize(this VerificationException exception)
        {
            return new Error(exception.Code, exception.Message);
        }
    }

}
