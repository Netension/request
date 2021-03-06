﻿using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Abstraction.Requests;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Http.Wrappers
{
    public class HttpCorrelationWrapper : IHttpRequestWrapper
    {
        private readonly ICorrelationAccessor _correlationAccessor;
        private readonly IHttpRequestWrapper _next;
        private readonly ILogger<HttpCorrelationWrapper> _logger;

        public HttpCorrelationWrapper(ICorrelationAccessor correlationAccessor, IHttpRequestWrapper next, ILogger<HttpCorrelationWrapper> logger)
        {
            _correlationAccessor = correlationAccessor;
            _next = next;
            _logger = logger;
        }

        public async Task<JsonContent> WrapAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            var response = await _next.WrapAsync(request, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Set {header} header to {correlationId}", CorrelationDefaults.CorrelationId, _correlationAccessor.CorrelationId);
            response.Headers.SetCorrelationId(_correlationAccessor.CorrelationId);

            _logger.LogDebug("Set {header} header to {causationId}", CorrelationDefaults.CausationId, _correlationAccessor.MessageId);
            response.Headers.SetCausationId(_correlationAccessor.MessageId);

            return response;
        }
    }
}
