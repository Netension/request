using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Netension.Request.Abstraction.Handlers;
using System.Collections.Generic;
using System.Linq;
using Netension.Request.Abstraction.Behaviors;
using System.Xml.Linq;
using System.Collections;

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

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));

            var handler = (dynamic)_serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                _logger.LogError("Handler not found for {type}", query.GetType().Name);
                throw new InvalidOperationException($"Handler not found for {query.GetType().Name}");
            }
            var attributes = handler.GetType().GetCustomAttributes(true);

            try
            {
                var preHandlerType = typeof(IEnumerable<>).MakeGenericType(typeof(IPreQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse)));
                var preHandlers = _serviceProvider.GetService(preHandlerType);
                if (!(preHandlers is null))
                {
                    foreach (var preHandler in (IEnumerable)preHandlers)
                    {
                        await ((dynamic)preHandler).PreHandleAsync((dynamic)query, attributes, cancellationToken);
                    }
                }

                var response = await handler.HandleAsync((dynamic)query, cancellationToken);

                var postHandlerType = typeof(IEnumerable<>).MakeGenericType(typeof(IPostQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse)));
                var postHandlers = _serviceProvider.GetService(postHandlerType);
                if (!(postHandlers is null))
                {
                    foreach (var postHandler in (IEnumerable)postHandlers)
                    {
                        await ((dynamic)postHandler).PostHandleAsync((dynamic)query, response, attributes, cancellationToken);
                    }
                }

                return response;
            }
            catch (Exception exception)
            {
                var failureHandlerType = typeof(IEnumerable<>).MakeGenericType(typeof(IFailureQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse)));
                var failureHandlers = _serviceProvider.GetService(failureHandlerType);
                if (!(failureHandlers is null))
                {
                    foreach (var failureHandler in (IEnumerable)failureHandlers)
                    {
                        await ((dynamic)failureHandler).FailHandleAsync((dynamic)query, exception, attributes, cancellationToken);
                    }
                }
                _logger.LogError("Exception during handle {query} query", query.GetType().Name);
                throw;
            }
        }
    }
}
