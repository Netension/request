using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Infrastructure.EFCore.Handlers;
using Netension.Request.Sample.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Requests
{
    public class SampleCommand : Command
    {
        public string Required { get; }

        public SampleCommand(string required)
        {
            Required = required;
        }
    }

    public class SampleCommandValidator : AbstractValidator<SampleCommand>
    {
        public SampleCommandValidator()
        {
            RuleFor(c => c.Required)
                .NotEmpty();
        }
    }

    public class SampleCommandHandler : TransactionalCommandHandler<SampleCommand, SampleDbContext>
    {
        public SampleCommandHandler(SampleDbContext context, IQuerySender querySender, ILogger<SampleCommandHandler> logger)
            : base(context, querySender, logger)
        {
        }

        protected async override Task HandleInternalAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            if (command.Required.Equals("Test")) throw new VerificationException(402, "Value is Test");

            Logger.LogInformation("SampleCommand handled!");
            Logger.LogInformation(await QueryAsnyc(new SampleQuery(), cancellationToken).ConfigureAwait(false));
        }
    }
}
