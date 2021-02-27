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
            _logger.LogDebug("{sender} resolved for {id} request", key, query.RequestId);

            var querySenderFactory = (Func<string, IQuerySender>)_serviceProvider.GetService(typeof(Func<string, IQuerySender>));
            var querySender = querySenderFactory(key);

            return await querySender.QueryAsync(query, cancellationToken);
        }

        async Task ICommandSender.SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) 
        {
            var key = _resolver.Resolve(command);
            _logger.LogDebug("{sender} resolved for {id} request", key, command.RequestId);

            var commandSenderFactory = (Func<string, ICommandSender>)_serviceProvider.GetService(typeof(Func<string, ICommandSender>));
            var commandSender = commandSenderFactory(key);
            await commandSender.SendAsync(command, cancellationToken);
        }
    }
}
