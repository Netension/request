using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Infrastructure.EFCore.Handlers
{
    [ExcludeFromCodeCoverage]
    public abstract class TransactionalQueryHandler<TQuery, TResponse, TContext> : QueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TContext : DbContext
    {
        protected TContext Context { get; }

        protected TransactionalQueryHandler(TContext context, ILogger logger) 
            : base(logger)
        {
            Context = context;
        }

        public override async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            if (!(Context.Database.CurrentTransaction is null)) return await HandleInternalAsync(query, cancellationToken);

            using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);
            using (Logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = transaction.TransactionId}))
            {
                try
                {
                    var response = await HandleInternalAsync(query, cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return response;
                }
                catch
                {
                    await transaction.RollbackAsync(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
                    throw;
                }
            }
        }

        protected abstract Task<TResponse> HandleInternalAsync(TQuery query, CancellationToken cancellationToken);
    }
}
