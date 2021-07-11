using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Handlers;
using Netension.Request.Sample.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class UpdateCommand : Command
    {
        public string OriginalValue { get; }
        public string NewValue { get; }

        public UpdateCommand(string newValue, string originalValue)
        {
            NewValue = newValue;
            OriginalValue = originalValue;
        }
    }

    public class UpdateCommandValidator : AbstractValidator<UpdateCommand>
    {
        public UpdateCommandValidator()
        {
            RuleFor(c => c.OriginalValue)
                   .NotEmpty();
            RuleFor(c => c.NewValue)
                .NotEmpty();
        }
    }

    public class UpdateCommandHandler : CommandHandler<UpdateCommand>
    {
        private readonly SampleContext _context;

        public UpdateCommandHandler(SampleContext context, IQuerySender querySender, ILogger<CreateCommandHandler> logger)
            : base(querySender, logger)
        {
            _context = context;
        }

        public override Task HandleAsync(UpdateCommand command, CancellationToken cancellationToken)
        {
            _context.Update(command.OriginalValue, command.NewValue);

            return Task.CompletedTask;
        }
    }
}
