using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.NetCore.Asp.Options;
using Netension.Request.NetCore.Asp.Wrappers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Senders
{
    public class HttpCommandSender : ICommandSender
    {
        private readonly HttpClient _client;
        private readonly IOptions<HttpCommandSenderOptions> _options;
        private readonly IHttpRequestWrapper _wrapper;
        private readonly ILogger<HttpCommandSender> _logger;

        public HttpCommandSender(HttpClient client, IOptions<HttpCommandSenderOptions> options, IHttpRequestWrapper wrapper, ILogger<HttpCommandSender> logger)
        {
            _client = client;
            _options = options;
            _wrapper = wrapper;
            _logger = logger;
        }

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) 
            where TCommand : ICommand
        {
            var options = _options.Value;

            var content = await _wrapper.WrapAsync(command, cancellationToken);

            _logger.LogDebug("Send {requestId} command to {url}", command.RequestId, $"{_client.BaseAddress}{options.Path}");
            await _client.PostAsync(options.Path, content, cancellationToken);
        }
    }
}
