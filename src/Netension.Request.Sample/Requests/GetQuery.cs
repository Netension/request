using Microsoft.Extensions.Logging;
using Netension.Request.Handlers;
using Netension.Request.Sample.Contexts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class GetQuery : Query<IEnumerable<string>>
    {
    }

    public class SampleQueryHandler : QueryHandler<GetQuery, IEnumerable<string>>
    {
        private readonly SampleContext _context;

        public SampleQueryHandler(SampleContext context, ILogger<SampleQueryHandler> logger)
            : base(logger)
        {
            _context = context;
        }

        public override Task<IEnumerable<string>> HandleAsync(GetQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Get());
        }
    }
}
