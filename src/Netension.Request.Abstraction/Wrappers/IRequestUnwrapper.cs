using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Wrappers
{
    public interface IRequestUnwrapper<TEnvelop>
    {
        Task<IRequest> UnwrapAsync(TEnvelop envelop, CancellationToken cancellationToken);
    }
}
