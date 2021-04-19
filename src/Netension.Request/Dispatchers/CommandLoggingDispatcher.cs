using Microsoft.Extensions.Logging;
using Netension.Extensions.Correlation;
using Netension.Extensions.Logging.Extensions;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Dispatchers
{
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

        public async Task DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            using (_logger.BeginScope(LoggingDefaults.CorrelationId, _correlation.CorrelationId))
            {
                using (_logger.BeginScope(LoggingDefaults.CausationId, _correlation.CausationId))
                {
                    await _next.DispatchAsync(command, cancellationToken);
                }
            }
        }
    }
}
