using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class SampleCommand : Command
    {
    }

    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        private readonly ILogger<SampleCommandHandler> _logger;

        public SampleCommandHandler(ILogger<SampleCommandHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{command} was called", "SampleCommand");
            return Task.CompletedTask;
        }
    }
}
