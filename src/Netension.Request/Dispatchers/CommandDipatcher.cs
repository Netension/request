using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Dispatch {id} {type}", command.RequestId, command.GetType().Name);
            _logger.LogTrace("Command object: {@command}", command);

            var requestHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            _logger.LogDebug("Look for {type} handler", requestHandlerType);

            var handler = _serviceProvider.GetService(requestHandlerType);
            if (handler == null)
            {
                _logger.LogError("Handler not found for {type}", requestHandlerType);
                throw new InvalidOperationException($"Handler not found for {requestHandlerType}");
            }

            try
            {
                await ((dynamic)handler).HandleAsync((dynamic)command, cancellationToken);
            }
            catch
            {
                _logger.LogError("Exception during handle {command} command", command.GetType().Name);
                throw;
            }
        }
    }
}
