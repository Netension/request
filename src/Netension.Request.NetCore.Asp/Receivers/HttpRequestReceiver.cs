using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.NetCore.Asp.Unwrappers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Receivers
{
    public class HttpRequestReceiver : IHttpRequestReceiver
    {
        private readonly IHttpRequestUnwrapper _unwrapper;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<HttpRequestReceiver> _logger;

        public HttpRequestReceiver(IHttpRequestUnwrapper unwrapper, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, ILogger<HttpRequestReceiver> logger)
        {
            _unwrapper = unwrapper;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
        }



        public async Task<IActionResult> ReceiveAsync(HttpRequest message, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Receive {id} request", message.GetHashCode());
            var request = await _unwrapper.UnwrapAsync(message, cancellationToken);

            if (request is ICommand command)
            {
                await _commandDispatcher.DispatchAsync(command, cancellationToken);
                return new AcceptedResult();
            }
            else if (request.GetType().GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQuery<>)))
            {
                return new OkObjectResult(await _queryDispatcher.DispatchAsync((dynamic)request, cancellationToken));
            }

            _logger.LogError("{requestType} message is unsupported", request.GetType().Name);
            throw new BadHttpRequestException($"{request.GetType().Name} message type is unsupported");
        }
    }
}
