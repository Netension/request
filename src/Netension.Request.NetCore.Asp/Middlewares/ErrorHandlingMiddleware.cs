using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Request.Http.Enumerations;
using Netension.Request.Http.ValueObjects;
using System;
using System.Collections.Generic;
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
                await _next.Invoke(context).ConfigureAwait(false);
            }
            catch (VerificationException exception)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["Exception"] = exception.GetType().Name,
                    ["Message"] = exception.Message
                }))
                {
                    _logger.LogError(exception, ErrorCodeEnumeration.VerificationError.Message);
                }

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.Body.WriteAsync(exception.Encode(), context.RequestAborted).ConfigureAwait(false);
            }
            catch (ValidationException exception)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["Exception"] = exception.GetType().Name,
                    ["Message"] = exception.Message
                }))
                {
                    _logger.LogError(exception, ErrorCodeEnumeration.ValidationFailed.Message);
                }
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.Body.WriteAsync(exception.Encode(), context.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["Exception"] = exception.GetType().Name,
                    ["Message"] = exception.Message
                }))
                {
                    _logger.LogError(exception, ErrorCodeEnumeration.InternalServerError.Message);
                }
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.Body.WriteAsync(exception.Encode(), context.RequestAborted).ConfigureAwait(false);
            }
        }
    }

    public static class ErrorHandlingExtensions
    {
        public static Error ToError(this ValidationException exception)
        {
            var message = new StringBuilder(ErrorCodeEnumeration.ValidationFailed.Message);
            message.AppendLine();

            foreach (var failure in exception.Errors)
            {
                message.Append("--").Append(failure.PropertyName).Append(": ").AppendLine(failure.ErrorMessage);
            }

            return new Error(ErrorCodeEnumeration.ValidationFailed.Id, message.ToString());
        }

        public static ReadOnlyMemory<byte> Encode(this ValidationException exception)
        {
            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(exception.ToError()));
        }

        public static ReadOnlyMemory<byte> Encode(this VerificationException exception)
        {
            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(new Error(exception.Code, exception.Message)));
        }

        public static ReadOnlyMemory<byte> Encode(this Exception exception)
        {
            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(new Error(ErrorCodeEnumeration.InternalServerError.Id, ErrorCodeEnumeration.InternalServerError.Message)));
        }
    }
}
