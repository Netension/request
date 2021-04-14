using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Netension.Request.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Receivers
{
    /// <summary>
    /// Reponsible for create scope for the <see cref="ILoopbackRequestReceiver"/>.
    /// </summary>
    public class LoopbackScopeHandler : ILoopbackRequestReceiver
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILoopbackRequestReceiver _next;
        private readonly ILogger<LoopbackScopeHandler> _logger;

        public LoopbackScopeHandler(IServiceScopeFactory serviceScopeFactory, ILoopbackRequestReceiver next, ILogger<LoopbackScopeHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _next = next;
            _logger = logger;
        }

        public async Task<object> ReceiveAsync(LoopbackMessage message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _logger.LogDebug("New scope created for loopback message");
            return await _next.ReceiveAsync(message, cancellationToken);
        }
    }
}
