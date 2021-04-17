using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Infrastructure.EFCore.Handlers
{
    public class TransactionCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _next;
        private readonly ILogger<TransactionCommandHandler<TCommand>> _logger;
        private readonly DatabaseFacade _database;

        public TransactionCommandHandler(DatabaseFacade database, ICommandHandler<TCommand> next, ILogger<TransactionCommandHandler<TCommand>> logger)
        {
            _next = next;
            _logger = logger;
            _database = database;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin transaction");
            using var transaction = await _database.BeginTransactionAsync(cancellationToken);

            try
            {
                await _next.HandleAsync(command, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
                throw;
            }
        }
    }
}
