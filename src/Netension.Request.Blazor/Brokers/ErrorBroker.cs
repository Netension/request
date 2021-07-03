using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Netension.Request.Blazor.ValueObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Brokers
{
    public class ErrorBroker : IErrorPublisher, IErrorChannel
    {
        private readonly ILogger<ErrorBroker> _logger;
        private HandleError _handlers;

        public ErrorBroker(ILogger<ErrorBroker> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(CancellationToken cancellationToken)
        {
            if (_handlers is null)
            {
                _logger.LogDebug("No subscribed error handler");
                return Task.CompletedTask;
            }

            _handlers.Invoke(new Error { Code = 500, Message = "Unexpected server error" }, cancellationToken);
            return Task.CompletedTask;
        }

        public Task PublishAsync(int code, string message, CancellationToken cancellationToken)
        {
            if (_handlers is null)
            {
                _logger.LogDebug("No subscribed error handler");
                return Task.CompletedTask;
            }

            _handlers.Invoke(new Error { Code = code, Message = message }, cancellationToken);
            return Task.CompletedTask;
        }

        public Task PublishAsync(IEnumerable<ValidationFailure> failures, CancellationToken cancellationToken)
        {
            if (_handlers is null)
            {
                _logger.LogDebug("No subscribed error handler");
                return Task.CompletedTask;
            }

            _handlers.Invoke(new Error { Code = 400, Message = "Validation exception" }, cancellationToken);
            return Task.CompletedTask;
        }

        public void Subscribe(HandleError handler) => _handlers += handler;
        public void Unsubscribe(HandleError handler) => _handlers -= handler;
    }
}
