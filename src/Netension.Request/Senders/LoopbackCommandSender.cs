using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Receivers;
using Netension.Request.Wrappers;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Senders
{
    /// <inheritdoc cref="ICommandSender"/>
    public class LoopbackCommandSender : ICommandSender
    {
        private readonly ILoopbackRequestWrapper _wrapper;
        private readonly ILoopbackRequestReceiver _receiver;
        private readonly ILogger<LoopbackCommandSender> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LoopbackCommandSender"/>.
        /// </summary>
        /// <param name="wrapper"><see cref="ILoopbackRequestWrapper"/> instance.</param>
        /// <param name="receiver"><see cref="ILoopbackRequestReceiver"/> instance.</param>
        /// <param name="logger"><see cref="ILogger{TCategoryName}"/> instance.</param>
        public LoopbackCommandSender(ILoopbackRequestWrapper wrapper, ILoopbackRequestReceiver receiver, ILogger<LoopbackCommandSender> logger)
        {
            _wrapper = wrapper;
            _receiver = receiver;
            _logger = logger;
        }

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            _logger.LogDebug("Send {id} command via {type} sender", command.RequestId, "loopback");

            var message = await _wrapper.WrapAsync(command, cancellationToken);
            await _receiver.ReceiveAsync(message, cancellationToken);
        }
    }
}
