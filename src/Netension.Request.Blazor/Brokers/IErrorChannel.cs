using Netension.Request.Blazor.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Brokers
{
    public delegate Task HandleError(Error erro, CancellationToken cancellationToken);

    public interface IErrorChannel
    {
        void Subscribe(HandleError handler);
        void Unsubscribe(HandleError handler);
    }
}
