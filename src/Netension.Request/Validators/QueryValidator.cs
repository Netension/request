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
    public class QueryValidator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        private readonly IEnumerable<IValidator<TQuery>> _validators;
        private readonly IQueryHandler<TQuery, TResponse> _next;
        private readonly ILogger<QueryValidator<TQuery, TResponse>> _logger;

        public QueryValidator(IEnumerable<IValidator<TQuery>> validators, IQueryHandler<TQuery, TResponse> next, ILogger<QueryValidator<TQuery, TResponse>> logger)
        {
            _validators = validators;
            _next = next;
            _logger = logger;
        }

        public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                _logger.LogDebug("Validate {id} query", query.RequestId);
                await query.ValidateAsync(_validators, cancellationToken);
                _logger.LogDebug("{id} query succesfully validated", query.RequestId);
            }
            else _logger.LogDebug("Validator not found for {id} query", query.RequestId);

            return await _next.HandleAsync(query, cancellationToken);
        }
    }
}
