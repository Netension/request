using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Receivers;
using Netension.Request.Wrappers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Senders
{
    /// <inheritdoc cref="IQuerySender"/>
    public class LoopbackQuerySender : IQuerySender
    {
        private readonly ILoopbackRequestWrapper _wrapper;
        private readonly ILoopbackRequestReceiver _receiver;
        private readonly ILogger<LoopbackQuerySender> _logger;

        /// <summary>
        /// Initialize new instance of <see cref="LoopbackQuerySender"/>.
        /// </summary>
        /// <param name="wrapper">Instance of <see cref="ILoopbackRequestWrapper"/>.</param>
        /// <param name="receiver">Instance of <see cref="ILoopbackRequestReceiver"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger{TCategoryName}"/>.</param>
        public LoopbackQuerySender(ILoopbackRequestWrapper wrapper, ILoopbackRequestReceiver receiver, ILogger<LoopbackQuerySender> logger)
        {
            _wrapper = wrapper;
            _receiver = receiver;
            _logger = logger;
        }

        public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            return QueryInternalAsync(query, cancellationToken);
        }

        private async Task<TResponse> QueryInternalAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Send {id} query via {sender} sender", query.RequestId, "loopback");

            var message = await _wrapper.WrapAsync(query, cancellationToken);
            return (TResponse)await _receiver.ReceiveAsync(message, cancellationToken);
        }
    }
}
