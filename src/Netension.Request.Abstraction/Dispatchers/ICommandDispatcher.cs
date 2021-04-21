using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Dispatchers
{
    /// <summary>
    /// Responsible for distribute the commands to the <see cref="Handlers.ICommandHandler{TCommand}">ICommandHandler</see>.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Consume the command from the channel and distribute it to the <see cref="Handlers.ICommandHandler{TCommand}">ICommandHandler</see>.
        /// </summary>
        /// <param name="command">Incoming <see cref="ICommand"/> instance.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <exception cref="System.InvalidOperationException">Throws if handler does not found for the incoming <see cref="ICommand"/>.</exception>
        Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand;
    }
}
