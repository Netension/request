using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Extensions;
using Netension.Request.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Wrappers
{
    /// <inheritdoc cref="ILoopbackRequestWrapper"/>
    public class LoopbackRequestWrapper : ILoopbackRequestWrapper
    {
        private readonly ILogger<LoopbackRequestWrapper> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LoopbackRequestWrapper"/>.
        /// </summary>
        /// <param name="logger"><see cref="ILogger{TCategoryName}"/> instance.</param>
        public LoopbackRequestWrapper(ILogger<LoopbackRequestWrapper> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Wrap <see cref="IRequest" /> to <see cref="LoopbackMessage"/>.
        /// </summary>
        /// <param name="request">Instance of <see cref="IRequest"/>.</param>
        /// <param name="cancellationToken"><inheritdoc/></param>
        /// <returns><see cref="LoopbackMessage"/> instance with wrapped <see cref="IRequest"/>.</returns>
        public Task<LoopbackMessage> WrapAsync(IRequest request, CancellationToken cancellationToken)
        {
            var message = new LoopbackMessage(request);

            message.Headers.SetMessageType(request.MessageType);
            _logger.LogDebug($"{RequestDefaults.Header.MessageType} headers setted up to {request.MessageType}");

            return Task.FromResult(message);
        }
    }
}
