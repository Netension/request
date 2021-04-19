using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Infrastructure.EFCore.Abstractions
{
    public interface ITransactionManager
    {
        object Id { get; }
        bool IsActive { get; }

        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitAsnyc(CancellationToken cancellationToken);
        Task RollbackAsync(CancellationToken cancellationToken);
    }
}
