using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Extensions;
using Netension.Request.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Wrappers
{
    /// <summary>
    /// Set Correlation-Id and Causation-Id header.
    /// </summary>
    public class LoopbackCorrelationWrapper : ILoopbackRequestWrapper
    {
        private readonly ICorrelationAccessor _correlationAccessor;
        private readonly ILoopbackRequestWrapper _next;
        private readonly ILogger<LoopbackCorrelationWrapper> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LoopbackRequestWrapper"/>.
        /// </summary>
        /// <param name="correlationAccessor"><see cref="ICorrelationAccessor"/> instance. Value of the Correlation-Id and Causation-Id will be getted via <paramref name="correlationAccessor"/>.</param>
        /// <param name="next">Next <see cref="ILoopbackRequestWrapper"/> instance in the pipeline.</param>
        /// <param name="logger"><see cref="ILogger{TCategoryName}"/> instance.</param>
        public LoopbackCorrelationWrapper(ICorrelationAccessor correlationAccessor, ILoopbackRequestWrapper next, ILogger<LoopbackCorrelationWrapper> logger)
        {
            _correlationAccessor = correlationAccessor;
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Wrap <see cref="IRequest"/> to <see cref="LoopbackMessage"/>.
        /// </summary>
        /// <param name="request"><inheritdoc/></param>
        /// <param name="cancellationToken"><inheritdoc/></param>
        /// <returns><see cref="LoopbackMessage"/> instance with the wrapped <see cref="IRequest"/>.</returns>
        public async Task<LoopbackMessage> WrapAsync(IRequest request, CancellationToken cancellationToken)
        {
            var message = await _next.WrapAsync(request, cancellationToken);

            var correlationId = _correlationAccessor.CorrelationId;
            _logger.LogDebug("Set {header} header to {correlationId}", CorrelationDefaults.CorrelationId, correlationId);
            message.Headers.SetCorrelationId(correlationId);

            _logger.LogDebug("Set {header} header to {causationId}", CorrelationDefaults.CausationId, _correlationAccessor.MessageId);
            message.Headers.SetCausationId(_correlationAccessor.MessageId);

            return message;
        }
    }
}
