using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Validators
{
    public class CommandValidator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        private readonly ICommandHandler<TCommand> _next;
        private readonly ILogger<CommandValidator<TCommand>> _logger;

        public CommandValidator(IEnumerable<IValidator<TCommand>> validators, ICommandHandler<TCommand> next, ILogger<CommandValidator<TCommand>> logger)
        {
            _validators = validators;
            _next = next;
            _logger = logger;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                _logger.LogDebug("Validate {id} command", command.RequestId);
                await command.ValidateAsync(_validators, cancellationToken);
                _logger.LogDebug("{id} command succesfully validated", command.RequestId);
            }
            else _logger.LogDebug("Validator not found for {id} command", command.RequestId);

            await _next.HandleAsync(command, cancellationToken);
        }
    }
}
