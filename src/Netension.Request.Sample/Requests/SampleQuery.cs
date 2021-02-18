using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Sample.Requests;
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
        private readonly IRequestSender _requestSender;

        public SampleQueryHandler(IRequestSender requestSender)
        {
            _requestSender = requestSender;
        }

        public async Task<string> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            await _requestSender.SendAsync(new SampleCommand(), cancellationToken);

            return "SampleQuery result";
        }
    }
}
