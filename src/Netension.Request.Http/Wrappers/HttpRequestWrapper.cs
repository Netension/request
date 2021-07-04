using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.Abstraction.Requests;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Http.Wrappers
{
    public class HttpRequestWrapper : IHttpRequestWrapper
    {
        private readonly IOptions<JsonSerializerOptions> _options;
        private readonly ILogger<HttpRequestWrapper> _logger;

        public HttpRequestWrapper(IOptions<JsonSerializerOptions> options, ILogger<HttpRequestWrapper> logger)
        {
            _options = options;
            _logger = logger;
        }

        public Task<JsonContent> WrapAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            _logger.LogDebug("Serialize {request} to Json", request.RequestId);
            var content = JsonContent.Create(request, options: _options.Value);

            _logger.LogDebug("Set {header} header to {type}", RequestDefaults.Header.MessageType, request.MessageType);
            content.Headers.SetMessageType(request);

            return Task.FromResult(content);
        }
    }
}
