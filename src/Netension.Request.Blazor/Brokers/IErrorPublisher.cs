using FluentValidation.Results;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Brokers
{
    public interface IErrorPublisher
    {
        Task PublishAsync(CancellationToken cancellationToken);
        Task PublishAsync(int code, string message, CancellationToken cancellationToken);
        Task PublishAsync(IEnumerable<ValidationFailure> failures, CancellationToken cancellationToken);
    }
}
