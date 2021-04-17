﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Annotations;
using Netension.Request.Handlers;
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

    [Transaction]
    public class SampleCommandHandler : CommandHandler<SampleCommand>
    {
        public SampleCommandHandler(IQuerySender querySender, ILogger<SampleCommandHandler> logger)
            : base(querySender, logger)
        {
        }

        public async override Task HandleAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            if (command.Required.Equals("Test")) throw new VerificationException(402, "Value is Test");

            Logger.LogInformation("SampleCommand handled!");
            Logger.LogInformation(await QueryAsnyc(new SampleQuery(), cancellationToken));
        }
    }
}
