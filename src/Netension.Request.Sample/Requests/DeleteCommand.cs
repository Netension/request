using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Handlers;
using Netension.Request.Sample.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class DeleteCommand : Command
    {
        public string Value { get; }

        public DeleteCommand(string value)
        {
            Value = value;
        }
    }

    public class DeleteCommandValidator : AbstractValidator<DeleteCommand>
    {
        public DeleteCommandValidator()
        {
            RuleFor(c => c.Value)
                .NotEmpty();
        }
    }

    public class DeleteCommandHandler : CommandHandler<DeleteCommand>
    {
        private readonly SampleContext _context;

        public DeleteCommandHandler(SampleContext context, IQuerySender querySender, ILogger<CreateCommandHandler> logger)
            : base(querySender, logger)
        {
            _context = context;
        }

        public override Task HandleAsync(DeleteCommand command, CancellationToken cancellationToken)
        {
            _context.Delete(command.Value);

            return Task.CompletedTask;
        }
    }
}
