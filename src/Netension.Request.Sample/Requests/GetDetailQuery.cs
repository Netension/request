using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Request.Handlers;
using Netension.Request.Sample.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class GetDetailQuery : Query<string>
    {
        public string Value { get; }

        public GetDetailQuery(string value)
        {
            Value = value;
        }
    }

    public class GetDetailQueryValidator : AbstractValidator<GetDetailQuery>
    {
        public GetDetailQueryValidator()
        {
            RuleFor(q => q.Value)
                .NotEmpty();
        }
    }

    public class GetDetailQueryHandler : QueryHandler<GetDetailQuery, string>
    {
        private readonly SampleContext _context;

        public GetDetailQueryHandler(SampleContext context, ILogger<GetDetailQueryHandler> logger)
            : base(logger)
        {
            _context = context;
        }

        public override Task<string> HandleAsync(GetDetailQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Get(query.Value));
        }
    }
}
