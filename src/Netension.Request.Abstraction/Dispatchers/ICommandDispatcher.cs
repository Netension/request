using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Dispatchers
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(ICommand command, CancellationToken cancellationToken);
    }
}
