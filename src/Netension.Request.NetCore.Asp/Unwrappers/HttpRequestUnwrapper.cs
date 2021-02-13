using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Unwrappers
{
    public class HttpRequestUnwrapper : IHttpRequestUnwrapper
    {
        private readonly ILogger<HttpRequestUnwrapper> _logger;

        public HttpRequestUnwrapper(ILogger<HttpRequestUnwrapper> logger)
        {
            _logger = logger;
        }

        public async Task<IRequest> UnwrapAsync(HttpRequest envelop, CancellationToken cancellationToken)
        {
            var messageType = envelop.Headers.GetMessageType();

            _logger.LogDebug("Unwrap {type} request", messageType);
            return (IRequest)await envelop.ReadFromJsonAsync(Type.GetType(messageType), cancellationToken);
        }
    }
}
