using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Behaviors;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Runtime.InteropServices;

namespace Netension.Request.Dispatchers
{
    /// <inheritdoc/>
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand
        {
            _logger.LogDebug("Dispatch {id} {type}", command.RequestId, command.GetType().Name);
            _logger.LogTrace("Command object: {@command}", command);

            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
            if (handler is null)
            {
                _logger.LogError("Handler not found for {command}", command.GetType().Name);
                throw new InvalidOperationException($"Handler not found for {command.GetType().Name}");
            }
            var attributes = handler.GetType().GetCustomAttributes(true);

            try
            {
                foreach (var preHandler in _serviceProvider.GetService<IEnumerable<IPreCommandHandler<TCommand>>>() ?? Enumerable.Empty<IPreCommandHandler<TCommand>>())
                {
                    await preHandler.PreHandleAsync(command, attributes, cancellationToken);
                }

                await handler.HandleAsync(command, cancellationToken);

                foreach (var postHandler in _serviceProvider.GetService<IEnumerable<IPostCommandHandler<TCommand>>>() ?? Enumerable.Empty<IPostCommandHandler<TCommand>>())
                {
                    await postHandler.PostHandleAsync(command, attributes, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                foreach (var failureHandler in _serviceProvider.GetService<IEnumerable<IFailureCommandHandler<TCommand>>>() ?? Enumerable.Empty<IFailureCommandHandler<TCommand>>())
                {
                    await failureHandler.FailHandleAsync(command, exception, attributes, cancellationToken);
                }
                _logger.LogError("Exception during handle {command} command", command.GetType().Name);
                throw;
            }
        }
    }
}
