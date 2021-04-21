using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Behaviors
{
    public interface IPreCommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task PreHandleAsync(TCommand command, object[] attributes, CancellationToken cancellationToken);
    }
}
