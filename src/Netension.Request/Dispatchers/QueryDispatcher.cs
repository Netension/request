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
                //foreach (var preHandler in _serviceProvider.GetService<IEnumerable<IPreQueryHandler<IQuery<TResponse>, TResponse>>>() ?? Enumerable.Empty<IPreQueryHandler<IQuery<TResponse>, TResponse>>())
                //{
                //    await preHandler.PreHandleAsync(query, attributes, cancellationToken);
                //}

                var response = await handler.HandleAsync((dynamic)query, cancellationToken);

                //foreach (var postHandler in _serviceProvider.GetService<IEnumerable<IPostQueryHandler<IQuery<TResponse>, TResponse>>>() ?? Enumerable.Empty<IPostQueryHandler<IQuery<TResponse>, TResponse>>())
                //{
                //    await postHandler.PostHandleAsync(query, response, attributes, cancellationToken);
                //}

                return response;
            }
            catch (Exception exception)
            {
                //foreach (var failureHandler in _serviceProvider.GetService<IEnumerable<IFailureQueryHandler<IQuery<TResponse>, TResponse>>>() ?? Enumerable.Empty<IFailureQueryHandler<IQuery<TResponse>, TResponse>>())
                //{
                //    await failureHandler.FailHandleAsync(query, exception, attributes, cancellationToken);
                //}
                //_logger.LogError("Exception during handle {query} query", query.GetType().Name);
                throw;
            }
        }
    }
}
