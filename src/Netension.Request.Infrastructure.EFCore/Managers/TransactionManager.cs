using Microsoft.EntityFrameworkCore.Infrastructure;
using Netension.Request.Infrastructure.EFCore.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Infrastructure.EFCore.Managers
{
    public class TransactionManager : ITransactionManager
    {
        private readonly DatabaseFacade _database;

        public object Id => _database.CurrentTransaction?.TransactionId;
        public bool IsActive => !(_database.CurrentTransaction is null);

        public TransactionManager(DatabaseFacade database)
        {
            _database = database;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await _database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsnyc(CancellationToken cancellationToken)
        {
            await _database.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            await _database.RollbackTransactionAsync(cancellationToken);
        }
    }
}
