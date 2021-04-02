using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Validators
{
    public class QueryValidator : IQueryDispatcher
    {
        public Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
