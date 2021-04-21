using Netension.Request.Abstraction.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Behaviors
{
    public interface IFailureCommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task FailHandleAsync(TCommand command, Exception exception, object[] attributes, CancellationToken cancellationToken);
    }
}
