using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Wrappers;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Senders
{
    public class LoopbackQuerySender : IQuerySender
    {
        private readonly ILoopbackRequestWrapper _wrapper;
        private readonly ILogger<LoopbackQuerySender> _logger;

        public LoopbackQuerySender(ILoopbackRequestWrapper wrapper, ILogger<LoopbackQuerySender> logger)
        {
            _wrapper = wrapper;
            _logger = logger;
        }

        public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Send {id} command via {sender} sender", query.RequestId, "loopback");

            var message = await _wrapper.WrapAsync(query, cancellationToken);

            return default(TResponse);
        }
    }
}
