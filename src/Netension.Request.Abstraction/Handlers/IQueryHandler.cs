using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Handlers
{
    /// <summary>
    /// Responsible for handle <see cref="IQuery{TResponse}"/>. Only an <see cref="IQueryHandler{TQuery, TResponse}"/> could be for each <see cref="IQuery{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of the handled <see cref="IQuery{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">Type of the result.</typeparam>
    public interface IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        /// <summary>
        /// Handle the <see cref="IQuery{TResponse}"/> instance.
        /// </summary>
        /// <param name="query">Instance of the incoming <see cref="IQuery{TResponse}"/>.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <returns>Result of the <see cref="IQuery{TResponse}"/>.</returns>
        Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
