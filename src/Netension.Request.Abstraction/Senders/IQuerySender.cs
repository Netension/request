using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Senders
{
    public interface IQuerySender
    {
        Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
    }
}
