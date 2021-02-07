using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Senders
{
    public interface ICommandSender
    {
        Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand;
    }
}
