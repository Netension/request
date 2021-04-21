using System.Threading;
using System;
using System.Threading.Tasks;
using Netension.Request.Abstraction.Requests;

namespace Netension.Request.Abstraction.Behaviors
{
    public interface IFailureQueryHandler<TQuery, in TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task FailHandleAsync(TQuery command, Exception exception, object[] attributes, CancellationToken cancellationToken);
    }
}
