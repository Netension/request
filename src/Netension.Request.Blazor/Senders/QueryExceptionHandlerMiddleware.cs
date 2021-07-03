using FluentValidation;
using Microsoft.Extensions.Logging;
using Netension.Core.Exceptions;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Blazor.Brokers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Senders
{
    public class QueryExceptionHandlerMiddleware : IQuerySender
    {
        private readonly IQuerySender _next;
        private readonly IErrorPublisher _errorHandler;
        private readonly ILogger<QueryExceptionHandlerMiddleware> _logger;

        public QueryExceptionHandlerMiddleware(IQuerySender next, IErrorPublisher errorHandler, ILogger<QueryExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            try
            {
                return await _next.QueryAsync(query, cancellationToken).ConfigureAwait(false);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation exception during handle {requestId} query", query.RequestId);
                await _errorHandler.PublishAsync(ex.Errors, cancellationToken).ConfigureAwait(false);

                return default;
            }
            catch (VerificationException ex)
            {
                _logger.LogError(ex, "Verification error during handle {requestID} query", query.RequestId);
                await _errorHandler.PublishAsync(ex.Code, ex.Message, cancellationToken).ConfigureAwait(false);

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during handle {requestId} query", query.RequestId);
                await _errorHandler.PublishAsync(cancellationToken).ConfigureAwait(false);

                return default;
            }
        }
    }
}
