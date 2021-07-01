using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Blazor.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Senders
{
    public class CommandSenderExecptionHandler : ICommandSender
    {
        private readonly ICommandSender _next;
        private readonly IErrorHandler _errorHandler;
        private readonly ILogger<CommandSenderExecptionHandler> _logger;

        public CommandSenderExecptionHandler(ICommandSender next, IErrorHandler errorHandler, ILogger<CommandSenderExecptionHandler> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during handle {requestId} command", command.RequestId);
                await _errorHandler.HandleErrorAsync(ex).ConfigureAwait(false);
            }
        }
    }
}
