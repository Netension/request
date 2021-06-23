using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Extensions.Logging.Extensions;
using Netension.Request.NetCore.Asp.Enumerations;
using Netension.Request.NetCore.Asp.ValueObjects;
using System;
using System.Collections.Generic;
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
            catch (NotFoundException exception)
            {
                _logger.LogError(exception, "Exception during processing request");
                await context.Response.PrepareResponse(StatusCodes.Status404NotFound, exception.GetBytes(), context.RequestAborted);
            }
            catch (ConflictException exception)
            {
                _logger.LogError(exception, "Exception during processing request");
                await context.Response.PrepareResponse(StatusCodes.Status409Conflict, exception.GetBytes(), context.RequestAborted);
            }
            catch (VerificationException exception)
            {
                _logger.LogError(exception, "Exception during processing request");
                await context.Response.PrepareResponse(StatusCodes.Status400BadRequest, exception.GetBytes(), context.RequestAborted);
            }
            catch (ValidationException exception)
            {
                _logger.LogError(exception, "Exception during processing request");
                await context.Response.PrepareResponse(StatusCodes.Status400BadRequest, exception.GetBytes(), context.RequestAborted);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception during processing request");
                await context.Response.PrepareResponse(StatusCodes.Status500InternalServerError, exception.GetBytes(), context.RequestAborted);
            }
        }
    }

    public static class ErrorHandlingExtensions
    {
        public static async Task PrepareResponse(this HttpResponse response, int statusCode, ReadOnlyMemory<byte> content, CancellationToken cancellationToken)
        {
            response.StatusCode = statusCode;
            response.ContentType = MediaTypeNames.Application.Json;
            await response.Body.WriteAsync(content, cancellationToken);
        }

        public static Error ToError(this ValidationException exception)
        {
            var message = new StringBuilder(ErrorCodeEnumeration.ValidationFailed.Message);
            message.AppendLine();

            foreach (var failure in exception.Errors)
            {
                message.AppendLine($"--{failure.PropertyName}: {failure.ErrorMessage}");
            }

            return new Error(ErrorCodeEnumeration.ValidationFailed.Id, message.ToString());
        }

        public static ReadOnlyMemory<byte> GetBytes(this Exception exception)
        {
            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(new Error(ErrorCodeEnumeration.InternalServerError.Id, ErrorCodeEnumeration.InternalServerError.Message)));
        }

        public static ReadOnlyMemory<byte> GetBytes(this VerificationException exception)
        {
            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(new Error(exception.Code, exception.Message)));
        }

        public static ReadOnlyMemory<byte> GetBytes(this ValidationException exception)
        {
            return new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(exception.ToError()));
        }
    }
}
