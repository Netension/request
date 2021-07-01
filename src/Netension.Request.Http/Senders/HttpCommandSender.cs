using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Http.Extensions;
using Netension.Request.Http.Options;
using Netension.Request.Http.Wrappers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Http.Senders
{
    public class HttpCommandSender : ICommandSender
    {
        private readonly HttpClient _client;
        private readonly HttpSenderOptions _options;
        private readonly IHttpRequestWrapper _wrapper;
        private readonly ILogger<HttpCommandSender> _logger;

        public HttpCommandSender(HttpClient client, HttpSenderOptions options, IHttpRequestWrapper wrapper, ILogger<HttpCommandSender> logger)
        {
            _client = client;
            _options = options;
            _wrapper = wrapper;
            _logger = logger;
        }

        public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            return SendInternalAsync(command, cancellationToken);
        }

        public async Task SendInternalAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand
        {
            var content = await _wrapper.WrapAsync(command, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Send {requestId} command to {url}", command.RequestId, $"{_client.BaseAddress}{_options.Path}");
            var response = await _client.PostAsync(_options.Path, content, cancellationToken).ConfigureAwait(false);
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw await response.Content.DeserializeBadRequestAsync(cancellationToken).ConfigureAwait(false);
                default:
                    response.EnsureSuccessStatusCode();
                    break;
            }
        }
    }
}
