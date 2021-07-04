using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Extensions;
using Netension.Request.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Unwrappers
{
    /// <summary>
    /// Unwrap Correlation-Id and Causation-Id from <see cref="ILoopbackRequestUnwrapper"/>.
    /// </summary>
    /// <remarks>
    /// The unwrapped Correlation-Id and Causation-Id value will be accessible via <see cref="ICorrelationAccessor"/>.
    /// </remarks>
    public class LoopbackCorrelationUnwrapper : ILoopbackRequestUnwrapper
    {
        private readonly ICorrelationMutator _correlationMutator;
        private readonly ILoopbackRequestUnwrapper _next;
        private readonly ILogger<LoopbackCorrelationUnwrapper> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LoopbackCorrelationUnwrapper"/>.
        /// </summary>
        /// <param name="correlationMutator"><see cref="ICorrelationMutator"/> instance.</param>
        /// <param name="next">Next <see cref="ILoopbackRequestUnwrapper"/> instance in the pipeline.</param>
        /// <param name="logger"><see cref="ILogger{TCategoryName}"/> instance.</param>
        public LoopbackCorrelationUnwrapper(ICorrelationMutator correlationMutator, ILoopbackRequestUnwrapper next, ILogger<LoopbackCorrelationUnwrapper> logger)
        {
            _correlationMutator = correlationMutator;
            _next = next;
            _logger = logger;
        }

        /// <inheritdoc/>
        /// <exception cref="System.InvalidOperationException">Throws if Correlation-Id header does not present.</exception>
        public async Task<IRequest> UnwrapAsync(LoopbackMessage envelop, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Set {property} to {requestId}", nameof(_correlationMutator.MessageId), envelop.Request.RequestId);
            _correlationMutator.MessageId = envelop.Request.RequestId.Value;

            _logger.LogDebug("Set {property} to {correlationId}", nameof(_correlationMutator.CorrelationId), envelop.Headers.GetCorrelationId());
            _correlationMutator.CorrelationId = envelop.Headers.GetCorrelationId();

            _logger.LogDebug("Set {property} to {causationId}", nameof(_correlationMutator.CausationId), envelop.Headers.GetCausationId());
            _correlationMutator.CausationId = envelop.Headers.GetCausationId();

            return await _next.UnwrapAsync(envelop, cancellationToken).ConfigureAwait(false);
        }
    }
}
