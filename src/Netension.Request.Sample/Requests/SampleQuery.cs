using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Sample.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class SampleQuery : Query<string>
    {
    }

    public class SampleQueryHandler : IQueryHandler<SampleQuery, string>
    {
        private readonly ICommandSender _sender;

        public SampleQueryHandler(ICommandSender sender)
        {
            _sender = sender;
        }

        public async Task<string> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            await _sender.SendAsync(new SampleCommand(), cancellationToken);

            return "SampleQuery result";
        }
    }
}
