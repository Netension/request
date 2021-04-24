using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Handlers
{
    [ExcludeFromCodeCoverage]
    public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        protected ILogger Logger { get; }

        protected QueryHandler(ILogger logger)
        {
            Logger = logger;
        }

        public abstract Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
