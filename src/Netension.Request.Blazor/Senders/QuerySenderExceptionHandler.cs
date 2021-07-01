using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Blazor.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Senders
{
    public class QuerySenderExceptionHandler : IQuerySender
    {
        private readonly IQuerySender _next;
        private readonly ILogger<QuerySenderExceptionHandler> _logger;

        public QuerySenderExceptionHandler(IQuerySender next, ILogger<QuerySenderExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            try
            {
                return await _next.QueryAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during handle {requestId} handle", query.RequestId);
                throw new RequestException(0, "Exception");
            }
        }
    }
}
