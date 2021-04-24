using Microsoft.Extensions.Logging;
using Netension.Request.Infrastructure.EFCore.Handlers;
using Netension.Request.Sample.Contexts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class SampleQuery : Query<string>
    {
    }

    public class SampleQueryHandler : TransactionalQueryHandler<SampleQuery, string, SampleDbContext>
    {
        public SampleQueryHandler(SampleDbContext context, ILogger<SampleQueryHandler> logger)
            : base(context, logger)
        {
        }

        protected override Task<string> HandleInternalAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult($"SampleQuery call at {DateTime.Now}");
        }
    }
}
