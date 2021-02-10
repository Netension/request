using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Extensions;
using Netension.Request.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Wrappers
{
    public class LoopbackCorrelationWrapper : ILoopbackRequestWrapper
    {
        private readonly ICorrelationAccessor _correlationAccessor;
        private readonly ILoopbackRequestWrapper _next;
        private readonly ILogger<LoopbackCorrelationWrapper> _logger;

        public LoopbackCorrelationWrapper(ICorrelationAccessor correlationAccessor, ILoopbackRequestWrapper next, ILogger<LoopbackCorrelationWrapper> logger)
        {
            _correlationAccessor = correlationAccessor;
            _next = next;
            _logger = logger;
        }

        public async Task<LoopbackMessage> WrapAsync(IRequest request, CancellationToken cancellationToken)
        {
            var message = await _next.WrapAsync(request, cancellationToken);

            var correlationId = _correlationAccessor.CorrelationId ?? Guid.NewGuid();
            _logger.LogDebug("Set {header} header to {correlationId}", CorrelationDefaults.CorrelationId, correlationId);
            message.Headers.SetCorrelationId(correlationId);

            var messageId = message.Request.RequestId.Value;
            _logger.LogDebug("Set {header} header to {causationId}", CorrelationDefaults.CausationId, messageId);
            message.Headers.SetCausationId(messageId);

            return message;
        }
    }
}
