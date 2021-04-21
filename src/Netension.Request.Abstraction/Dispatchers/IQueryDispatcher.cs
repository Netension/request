using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Dispatchers
{
    /// <summary>
    /// Responsible for distribute the queries to the <see cref="Handlers.IQueryHandler{TQuery, TResponse}">IQueryHandler</see>.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Consume the query from the channel and distribute it to the <see cref="Handlers.IQueryHandler{TQuery, TResponse}">IQueryHandler</see>.
        /// </summary>
        /// <param name="query">Incoming <see cref="IQuery{TResponse}"/> instance.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <returns>Result of the incoming <see cref="IQuery{TResponse}"/></returns>
        /// <exception cref="System.InvalidOperationException">Throws if handler does not found for the incoming <see cref="IQuery{TResponse}"/>.</exception>
        Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
            where TQuery : IQuery<TResponse>;
    }
}
