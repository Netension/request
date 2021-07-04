using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Infrastructure.EFCore.Handlers
{
    [ExcludeFromCodeCoverage]
    public abstract class TransactionalCommandHandler<TCommand, TContext> : CommandHandler<TCommand>
        where TCommand : ICommand
        where TContext : DbContext
    {
        protected TContext Context { get; }

        protected TransactionalCommandHandler(TContext context, IQuerySender querySender, ILogger logger)
            : base(querySender, logger)
        {
            Context = context;
        }

        public override async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (!(Context.Database.CurrentTransaction is null)) await HandleInternalAsync(command, cancellationToken).ConfigureAwait(false);

            using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            using (Logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = transaction.TransactionId }))
            {
                try
                {
                    await HandleInternalAsync(command, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await transaction.RollbackAsync(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token).ConfigureAwait(false);
                    throw;
                }
            }
        }

        protected abstract Task HandleInternalAsync(TCommand command, CancellationToken cancellationToken);
    }
}
