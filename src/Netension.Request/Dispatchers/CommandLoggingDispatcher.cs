using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Extensions.Logging.Extensions;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Dispatchers
{
    [ExcludeFromCodeCoverage]
    public class CommandLoggingDispatcher : ICommandDispatcher
    {
        private readonly ICorrelationAccessor _correlation;
        private readonly ICommandDispatcher _next;
        private readonly ILogger<CommandLoggingDispatcher> _logger;

        public CommandLoggingDispatcher(ICorrelationAccessor correlation, ICommandDispatcher next, ILogger<CommandLoggingDispatcher> logger)
        {
            _correlation = correlation;
            _next = next;
            _logger = logger;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand
        {
            using (_logger.BeginScope(LoggingDefaults.CorrelationId, _correlation.CorrelationId))
            {
                using (_logger.BeginScope(LoggingDefaults.CausationId, _correlation.CausationId))
                {
                    await _next.DispatchAsync(command, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
