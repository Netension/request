using Microsoft.Extensions.Logging;
using Netension.Extensions.Logging.Extensions;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Infrastructure.EFCore.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Infrastructure.EFCore.Handlers
{
    public class TransactionalCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ITransactionManager _transactionManager;
        private readonly ICommandHandler<TCommand> _next;
        private readonly ILogger<TransactionalCommandHandler<TCommand>> _logger;

        public TransactionalCommandHandler(ITransactionManager transactionManager, ICommandHandler<TCommand> next, ILogger<TransactionalCommandHandler<TCommand>> logger)
        {
            _transactionManager = transactionManager;
            _next = next;
            _logger = logger;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (_transactionManager.IsActive) return;

            await _transactionManager.BeginTransactionAsync(cancellationToken);
            using (_logger.BeginScope(LoggingDefaults.TransactionId, _transactionManager.Id))
            {
                try
                {
                    await _next.HandleAsync(command, cancellationToken);
                    await _transactionManager.CommitAsnyc(cancellationToken);
                }
                catch
                {
                    await _transactionManager.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }
}
