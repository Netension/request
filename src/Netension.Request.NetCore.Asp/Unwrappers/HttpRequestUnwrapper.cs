using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Defaults;
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
            _logger.LogDebug("Unwrap {id} request", envelop.GetHashCode());
            var messageType = envelop.Headers.GetMessageType();

            if (messageType == null)
            {
                _logger.LogError("{header} header not present", RequestDefaults.Header.MessageType);
                throw new BadHttpRequestException($"{RequestDefaults.Header.MessageType} header not present");
            }

            _logger.LogDebug("Unwrap {type} request", messageType);
            return (IRequest)await envelop.ReadFromJsonAsync(Type.GetType(messageType), cancellationToken);
        }
    }
}
