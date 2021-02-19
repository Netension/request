using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Unwrappers
{
    public class HttpCorrelationUnwrapper : IHttpRequestUnwrapper
    {
        private readonly ICorrelationMutator _correlationMutator;
        private readonly IHttpRequestUnwrapper _next;
        private readonly ILogger<HttpCorrelationUnwrapper> _logger;

        public HttpCorrelationUnwrapper(ICorrelationMutator correlationMutator, IHttpRequestUnwrapper next, ILogger<HttpCorrelationUnwrapper> logger)
        {
            _correlationMutator = correlationMutator;
            _next = next;
            _logger = logger;
        }

        public async Task<IRequest> UnwrapAsync(HttpRequest envelop, CancellationToken cancellationToken)
        {
            var request = await _next.UnwrapAsync(envelop, cancellationToken);

            var correlationId = envelop.Headers.GetCorrelationId();
            _logger.LogDebug("Set {property} to {id}", nameof(_correlationMutator.CorrelationId), correlationId);
            _correlationMutator.CorrelationId = correlationId;

            var causationId = envelop.Headers.GetCausationId();
            _logger.LogDebug("Set {property} to {id}", nameof(_correlationMutator.CausationId), causationId);
            _correlationMutator.CausationId = causationId;

            _logger.LogDebug("Set {property} to {id}", nameof(_correlationMutator.MessageId), request.RequestId);
            _correlationMutator.MessageId = request.RequestId.Value;

            return request;
        }
    }
}
