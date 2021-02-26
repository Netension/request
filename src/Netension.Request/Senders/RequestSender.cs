using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Resolvers;
using Netension.Request.Abstraction.Senders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Senders
{
    public class RequestSender : IRequestSender
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

        public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            var keys = _resolver.Resolve(query);

            foreach (var key in keys)
            {
                _logger.LogDebug("Send {id} request to {key} sender", query.RequestId, key);
                var querySenderFactory = (Func<string, IQuerySender>)_serviceProvider.GetService(typeof(Func<string, IQuerySender>));
                var querySender = querySenderFactory(key);
                return await querySender.QueryAsync(query, cancellationToken);
            }

            return default(TResponse);
        }

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            var keys = _resolver.Resolve(command);

            foreach (var key in keys)
            {
                _logger.LogDebug("Send {id} request to {key} sender", command.RequestId, key);
                var commandSenderFactory = (Func<string, ICommandSender>)_serviceProvider.GetService(typeof(Func<string, ICommandSender>));
                var commandSender = commandSenderFactory(key);
                await commandSender.SendAsync(command, cancellationToken);
            }
        }
    }
}
