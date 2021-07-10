using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Blazor.Brokers;
using Netension.Request.Http.Enumerations;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Senders
{
    public class CommandExceptionHandlerMiddleware : ICommandSender
    {
        private readonly ICommandSender _next;
        private readonly IErrorPublisher _errorPublisher;
        private readonly ILogger<CommandExceptionHandlerMiddleware> _logger;

        public CommandExceptionHandlerMiddleware(ICommandSender next, IErrorPublisher errorPublisher, ILogger<CommandExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _errorPublisher = errorPublisher;
            _logger = logger;
        }

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            try
            {
                await _next.SendAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation error during handle {requestId} command", command.RequestId);
                await _errorPublisher.PublishAsync(ex.Errors, cancellationToken).ConfigureAwait(false);
            }
            catch (VerificationException ex)
            {
                _logger.LogError(ex, "Verification error during handle {requestId} command", command.RequestId);
                await _errorPublisher.PublishAsync(ex.Code, ex.Message, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during handle {requestId} command", command.RequestId);
                await _errorPublisher.PublishAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
