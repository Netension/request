using Netension.Request.Abstraction.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class SampleQuery : Query<string>
    {
    }

    public class SampleQueryHandler : IQueryHandler<SampleQuery, string>
    {
        public Task<string> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult($"SampleQuery call at {DateTime.Now}");
        }
    }
}
