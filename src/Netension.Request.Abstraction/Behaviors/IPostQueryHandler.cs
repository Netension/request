using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Behaviors
{
    public interface IPostQueryHandler<TQuery, in TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task PostHandleAsync(TQuery query, TResponse response, object[] attributes, CancellationToken cancellationToken);
    }
}
