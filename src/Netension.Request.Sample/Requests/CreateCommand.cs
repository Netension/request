using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Handlers;
using Netension.Request.Sample.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class CreateCommand : Command
    {
        public string Value { get; }

        public CreateCommand(string value)
        {
            Value = value;
        }
    }

    public class CreateCommandValidator : AbstractValidator<CreateCommand>
    {
        public CreateCommandValidator()
        {
            RuleFor(c => c.Value)
                .NotEmpty();
        }
    }

    public class CreateCommandHandler : CommandHandler<CreateCommand>
    {
        private readonly SampleContext _context;

        public CreateCommandHandler(SampleContext context, IQuerySender querySender, ILogger<CreateCommandHandler> logger)
            : base(querySender, logger)
        {
            _context = context;
        }

        public override Task HandleAsync(CreateCommand command, CancellationToken cancellationToken)
        {
            _context.Add(command.Value);

            return Task.CompletedTask;
        }
    }
}
