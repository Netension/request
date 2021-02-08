using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Defaults;
using Netension.Request.Messages;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Wrappers
{
    public class LoopbackRequestWrapper : ILoopbackRequestWrapper
    {
        private readonly ILogger<LoopbackRequestWrapper> _logger;

        public LoopbackRequestWrapper(ILogger<LoopbackRequestWrapper> logger)
        {
            _logger = logger;
        }

        public Task<LoopbackMessage> WrapAsync(IRequest request, CancellationToken cancellationToken)
        {
            var message = new LoopbackMessage(request);

            message.Headers.SetMessageType(request.MessageType);
            _logger.LogDebug($"{RequestDefaults.Header.MessageType} headers setted up to {request.MessageType}");

            return Task.FromResult(message);
        }
    }
}
