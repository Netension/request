using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Resolvers;
using Netension.Request.Abstraction.Senders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Senders
{
    public class RequestSender : ICommandSender, IQuerySender
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRequestSenderKeyResolver _resolver;
        private readonly ILogger<RequestSender> _logger;

        public RequestSender(IServiceProvider serviceProvider, IRequestSenderKeyResolver resolver, ILogger<RequestSender> logger)
        {
            _serviceProvider = serviceProvider;
            _resolver = resolver;
            _logger = logger;
        }

        async Task<TResponse> IQuerySender.QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            var key = _resolver.Resolve(query);
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogError("Sender was not found for {id} request", query.RequestId);
                throw new InvalidOperationException($"Sender was not found for {query.RequestId} request.");
            }
            _logger.LogDebug("{sender} resolved for {id} request", key, query.RequestId);

            var querySenderFactory = (Func<string, IQuerySender>)_serviceProvider.GetService(typeof(Func<string, IQuerySender>));
            var querySender = querySenderFactory(key);

            return await querySender.QueryAsync(query, cancellationToken).ConfigureAwait(false);
        }

        async Task ICommandSender.SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            var key = _resolver.Resolve(command);
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogError("Sender was not found for {id} request", command.RequestId);
                throw new InvalidOperationException($"Sender was not found for {command.RequestId} request.");
            }

            _logger.LogDebug("{sender} resolved for {id} request", key, command.RequestId);

            var commandSenderFactory = (Func<string, ICommandSender>)_serviceProvider.GetService(typeof(Func<string, ICommandSender>));
            var commandSender = commandSenderFactory(key);
            await commandSender.SendAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}
