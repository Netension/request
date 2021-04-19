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
    public class TransactionQueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IQueryHandler<TQuery, TResponse> _next;
        private readonly ILogger<TransactionQueryHandler<TQuery, TResponse>> _logger;

        public TransactionQueryHandler(ITransactionManager transactionManager, IQueryHandler<TQuery, TResponse> next, ILogger<TransactionQueryHandler<TQuery, TResponse>> logger)
        {
            _transactionManager = transactionManager;
            _next = next;
            _logger = logger;
}

        public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            if (_transactionManager.IsActive)
            {
                return await _next.HandleAsync(query, cancellationToken);
            }

            await _transactionManager.BeginTransactionAsync(cancellationToken);

            using (_logger.BeginScope(LoggingDefaults.TransactionId, _transactionManager.Id))
            {
                try
                {
                    var response = await _next.HandleAsync(query, cancellationToken);
                    await _transactionManager.CommitAsnyc(cancellationToken);

                    return response;
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
