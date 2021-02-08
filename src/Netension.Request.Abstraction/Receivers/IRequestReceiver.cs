using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Receivers
{
    public interface IRequestReceiver<TMessage, TResponse>
    {
        Task<TResponse> ReceiveAsync(TMessage message, CancellationToken cancellationToken);
    }
}
