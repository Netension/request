using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Behaviors
{
    public interface IPreQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<TResponse> PreHandleAsync(TQuery query, object[] attributes, CancellationToken cancellationToken);
    }
}
