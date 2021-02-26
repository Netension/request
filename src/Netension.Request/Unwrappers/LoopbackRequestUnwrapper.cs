using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Unwrappers
{
    /// <inheritdoc cref="ILoopbackRequestUnwrapper"/>
    public class LoopbackRequestUnwrapper : ILoopbackRequestUnwrapper
    {
        private readonly ILogger<LoopbackRequestUnwrapper> _logger;

        /// <inheritdoc/>
        /// <param name="logger"><see cref="ILogger{TCategoryName}"/> instance.</param>
        public LoopbackRequestUnwrapper(ILogger<LoopbackRequestUnwrapper> logger)
        {
            _logger = logger;
        }

        public Task<IRequest> UnwrapAsync(LoopbackMessage envelop, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Unwrap {requestId} request", envelop.Request);

            return Task.FromResult(envelop.Request);
        }
    }
}
