using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Extensions;
using Netension.Request.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Unwrappers
{
    public class LoopbackCorrelationUnwrapper : ILoopbackRequestUnwrapper
    {
        private ICorrelationMutator _correlationMutator;
        private ILoopbackRequestUnwrapper _next;
        private ILogger<LoopbackCorrelationUnwrapper> _logger;

        public LoopbackCorrelationUnwrapper(ICorrelationMutator correlationMutator, ILoopbackRequestUnwrapper next, ILogger<LoopbackCorrelationUnwrapper> logger)
        {
            _correlationMutator = correlationMutator;
            _next = next;
            _logger = logger;
        }

        public async Task<IRequest> UnwrapAsync(LoopbackMessage envelop, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Set {property} to {requestId}", nameof(_correlationMutator.MessageId), envelop.Request.RequestId);
            _correlationMutator.MessageId = envelop.Request.RequestId.Value;

            _logger.LogDebug("Set {property} to {correlationId}", nameof(_correlationMutator.CorrelationId), envelop.Headers.GetCorrelationId());
            _correlationMutator.CorrelationId = envelop.Headers.GetCorrelationId();

            _logger.LogDebug("Set {property} to {causationId}", nameof(_correlationMutator.CausationId), envelop.Headers.GetCausationId());
            _correlationMutator.CausationId = envelop.Headers.GetCausationId();

            return await _next.UnwrapAsync(envelop, cancellationToken);
        }
    }
}
