using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Http.Enumerations;
using Netension.Request.Http.Extensions;
using Netension.Request.Http.Options;
using Netension.Request.Http.Wrappers;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Http.Senders
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

        public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            return QueryInternalAsync(query, cancellationToken);
        }

        private async Task<TResponse> QueryInternalAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            var content = await _wrapper.WrapAsync(query, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Send {requestId} query to {url}", query.RequestId, $"{_client.BaseAddress}{_options.Path}");
            var response = await _client.PostAsync(_options.Path, content, cancellationToken).ConfigureAwait(false);

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw await response.Content.DeserializeBadRequestAsync(cancellationToken).ConfigureAwait(false);
                case HttpStatusCode.NotFound:
                    throw new VerificationException(ErrorCodeEnumeration.NotFound.Id, ErrorCodeEnumeration.NotFound.Message);
                case HttpStatusCode.Unauthorized:
                    throw new VerificationException(ErrorCodeEnumeration.Unathorized.Id, ErrorCodeEnumeration.Unathorized.Message);
                case HttpStatusCode.Forbidden:
                    throw new VerificationException(ErrorCodeEnumeration.Forbidden.Id, ErrorCodeEnumeration.Forbidden.Message);
                case HttpStatusCode.Conflict:
                    throw new VerificationException(ErrorCodeEnumeration.Conflict.Id, ErrorCodeEnumeration.Conflict.Message);
                default:
                    response.EnsureSuccessStatusCode();
                    break;
            }

            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
