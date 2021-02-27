using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Dispatchers
{
    /// <inheritdoc/>
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueryDispatcher> _logger;

        public QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Dispatch {id} {type}", query.RequestId, query.GetType().Name);
            _logger.LogTrace("Query object: {@query}", query);

            var requestHandlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
            _logger.LogDebug("Look for {type} handler", requestHandlerType);

            var handler = _serviceProvider.GetService(requestHandlerType);
            if (handler == null)
            {
                _logger.LogError("Handler not found for {type}", requestHandlerType);
                throw new InvalidOperationException($"Handler not found for {requestHandlerType}");
            }

            try
            {
                return await ((dynamic)handler).HandleAsync((dynamic)query, cancellationToken);
            }
            catch
            {
                _logger.LogError("Exception during handle {query} query", query.GetType().Name);
                throw;
            }
        }
    }
}
