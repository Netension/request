using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Handlers
{
    [ExcludeFromCodeCoverage]
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IQuerySender _querySender;
        protected ILogger Logger { get; }

        protected CommandHandler(IQuerySender querySender, ILogger logger)
        {
            _querySender = querySender;
            Logger = logger;
        }

        public abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken);

        protected async Task<TResponse> QueryAsnyc<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            return await _querySender.QueryAsync(query, cancellationToken).ConfigureAwait(false);
        }
    }
}
