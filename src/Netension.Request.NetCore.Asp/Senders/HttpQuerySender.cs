using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.NetCore.Asp.Options;
using Netension.Request.NetCore.Asp.Wrappers;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Senders
{
    public class HttpQuerySender : IQuerySender
    {
        private readonly HttpClient _client;
        private readonly HttpSenderOptions _options;
        private readonly IHttpRequestWrapper _wrapper;
        private readonly ILogger<HttpQuerySender> _logger;

        public HttpQuerySender(HttpClient client, HttpSenderOptions options, IHttpRequestWrapper wrapper, ILogger<HttpQuerySender> logger)
        {
            _client = client;
            _options = options;
            _wrapper = wrapper;
            _logger = logger;
        }

        public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var content = await _wrapper.WrapAsync(query, cancellationToken);

            _logger.LogDebug("Send {requestId} query to {url}", query.RequestId, $"{_client.BaseAddress}{_options.Path}");
            var response = await _client.PostAsync(_options.Path, content, cancellationToken);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
        }
    }
}
