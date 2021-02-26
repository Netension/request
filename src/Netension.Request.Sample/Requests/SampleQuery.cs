using Microsoft.Extensions.Logging;
using Netension.Request.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class SampleQuery : Query<string>
    {
    }

    public class SampleQueryHandler : QueryHandler<SampleQuery, string>
    {
        public SampleQueryHandler(ILogger<SampleQueryHandler> logger) 
            : base(logger)
        {
        }

        public override Task<string> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult($"SampleQuery call at {DateTime.Now}");
        }
    }
}
