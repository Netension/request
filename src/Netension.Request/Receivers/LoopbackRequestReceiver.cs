using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Messages;
using Netension.Request.Unwrappers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Receivers
{
    /// <inheritdoc/>
    public class LoopbackRequestReceiver : ILoopbackRequestReceiver
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILoopbackRequestUnwrapper _unwrapper;
        private readonly ILogger<LoopbackRequestReceiver> _logger;

        public LoopbackRequestReceiver(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, ILoopbackRequestUnwrapper unwrapper, ILogger<LoopbackRequestReceiver> logger)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _unwrapper = unwrapper;
            _logger = logger;
        }

        public async Task<object> ReceiveAsync(LoopbackMessage message, CancellationToken cancellationToken)
        {
            var request = await _unwrapper.UnwrapAsync(message, cancellationToken);
            _logger.LogDebug("Receive {requestType} request", request.GetType().Name);

            if (request is ICommand command)
            {
                await _commandDispatcher.DispatchAsync(command, cancellationToken);
                return null;
            }
            else if (request.GetType().GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQuery<>)))
            {
                return await _queryDispatcher.DispatchAsync((dynamic)request, cancellationToken);
            }

            return null;
        }
    }
}
