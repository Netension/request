using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Senders
{
    /// <summary>
    /// Responsible for wrap (by <see cref="Wrappers.IRequestWrapper{TEnvelop}">IRequestWrapper</see>) and send <see cref="IQuery{TResponse}"/>.
    /// </summary>
    public interface IQuerySender
    {
        /// <summary>
        /// Send the incoming <see cref="IQuery{TResponse}"/>.
        /// </summary>
        /// <typeparam name="TResponse">Type of the <see cref="IQuery{TResponse}"/>'s result.</typeparam>
        /// <param name="query">Instance of the <see cref="ICommand"/> to be sent.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <returns>Result of the <see cref="IQuery{TResponse}"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is null.</exception>
        Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
    }
}
