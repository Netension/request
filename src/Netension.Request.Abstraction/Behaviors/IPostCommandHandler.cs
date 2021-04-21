using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Behaviors
{
    public interface IPostCommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task PostHandleAsync(TCommand command, object[] attributes, CancellationToken cancellationToken);
    }
}
