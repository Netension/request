using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Handlers
{
    /// <summary>
    /// Responsible for handle <see cref="ICommand"/>. Only an <see cref="ICommandHandler{TCommand}"/> could be for each <see cref="ICommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the handled <see cref="ICommand"/>.</typeparam>
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Handle the <see cref="ICommand"/> instance.
        /// </summary>
        /// <param name="command">Instance of the incoming <see cref="ICommand"/>.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
