using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Handlers
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
