using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Wrappers
{
    public interface IRequestWrapper<TEnvelop>
    {
        Task<TEnvelop> WrapAsync(IRequest request, CancellationToken cancellationToken);
    }
}
