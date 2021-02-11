using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.NetCore.Asp.Options;
using Netension.Request.NetCore.Asp.Wrappers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Senders
{
    public class HttpQuerySender : IQuerySender
    {
        private readonly HttpClient _client;
        private readonly IOptions<JsonSerializerOptions> _serializerOptions;
        private readonly IOptions<HttpSenderOptions> _options;
        private readonly IHttpRequestWrapper _wrapper;
        private readonly ILogger<HttpQuerySender> _logger;

        public HttpQuerySender(HttpClient client, IOptions<JsonSerializerOptions> serializerOptions, IOptions<HttpSenderOptions> options, IHttpRequestWrapper wrapper, ILogger<HttpQuerySender> logger)
        {
            _client = client;
            _serializerOptions = serializerOptions;
            _options = options;
            _wrapper = wrapper;
            _logger = logger;
        }

        public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken) 
        {
            var options = _options.Value;

            var content = await _wrapper.WrapAsync(query, cancellationToken);

            _logger.LogDebug("Send {requestId} query to {url}", query.RequestId, $"{_client.BaseAddress}{options.Path}");
            var response = await _client.PostAsync(options.Path, content, cancellationToken);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(_serializerOptions.Value, cancellationToken);
        }
    }
}
