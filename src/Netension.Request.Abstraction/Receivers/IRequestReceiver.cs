using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Receivers
{
    /// <summary>
    /// Receive message from the channel.
    /// </summary>
    /// <typeparam name="TMessage">Type of the incoming message. It could be <see cref="Requests.ICommand">ICommand</see> or <see cref="Requests.IQuery{TResponse}">IQuery</see></typeparam>
    /// <typeparam name="TResponse">Type of the result of the incoming message. It is used in case of <see cref="Requests.IQuery{TResponse}">IQuery</see>.</typeparam>
    /// <remarks>
    /// Responsible for unwrap the incoming message by <see cref="Wrappers.IRequestUnwrapper{TEnvelop}">IRequestUnwrapper</see> and distribute it by <see cref="Dispatchers.ICommandDispatcher">ICommandDispatcher</see> or <see cref="Dispatchers.IQueryDispatcher">IQueryDispatcher</see>.
    /// </remarks>
    public interface IRequestReceiver<TMessage, TResponse>
    {
        /// <summary>
        /// Receive the incoming message.
        /// </summary>
        /// <param name="message">Instance of the message.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <returns>Result of the message. It is used in case of <see cref="Requests.IQuery{TResponse}">IQuery</see>.</returns>
        Task<TResponse> ReceiveAsync(TMessage message, CancellationToken cancellationToken);
    }
}
