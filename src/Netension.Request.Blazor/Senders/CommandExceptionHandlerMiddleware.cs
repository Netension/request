using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Blazor.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Senders
{
    public class CommandExceptionHandlerMiddleware : ICommandSender
    {
        private readonly ICommandSender _next;
        private readonly IErrorHandler _errorHandler;
        private readonly ILogger<CommandExceptionHandlerMiddleware> _logger;

        public CommandExceptionHandlerMiddleware(ICommandSender next, IErrorHandler errorHandler, ILogger<CommandExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            try
            {
                await _next.SendAsync(command, cancellationToken);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation error during handle {requestId} command", command.RequestId);
                await _errorHandler.HandleValidationErrorAsync(ex.Errors, cancellationToken);
            }
            catch (VerificationException ex)
            {
                _logger.LogError(ex, "Verification error during handle {requestId} command", command.RequestId);
                await _errorHandler.HandleVerificationErrorAsync(ex.Code, ex.Message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during handle {requestId} command", command.RequestId);
                await _errorHandler.HandleServerErrorAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
