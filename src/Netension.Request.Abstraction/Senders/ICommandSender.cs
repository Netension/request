using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Senders
{
    /// <summary>
    /// Responsible for wrap (by <see cref="Wrappers.IRequestWrapper{TEnvelop}">IRequestWrapper</see>) and send <see cref="ICommand"/>.
    /// </summary>
    public interface ICommandSender
    {
        /// <summary>
        /// Send the incoming <see cref="ICommand"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command. Must be implemented <see cref="ICommand"/> interface.</typeparam>
        /// <param name="command">Instance of the <see cref="ICommand"/> to be sent.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is null.</exception>
        Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand;
    }
}
