using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Extensions.Logging.Extensions;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Dispatchers
{
    public class QueryLoggingDispatcher : IQueryDispatcher
    {
        private readonly ICorrelationAccessor _correlation;
        private readonly IQueryDispatcher _next;
        private readonly ILogger<QueryLoggingDispatcher> _logger;

        public QueryLoggingDispatcher(ICorrelationAccessor correlation, IQueryDispatcher next, ILogger<QueryLoggingDispatcher> logger)
        {
            _correlation = correlation;
            _next = next;
            _logger = logger;
        }


        public async Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            using (_logger.BeginScope(LoggingDefaults.CorrelationId, _correlation.CorrelationId))
            {
                using (_logger.BeginScope(LoggingDefaults.CausationId, _correlation.CausationId))
                {
                    return await _next.DispatchAsync(query, cancellationToken);
                }
            }
        }
    }
}
