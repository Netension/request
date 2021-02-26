using Netension.Request.Abstraction.Requests;
using Netension.Request.Requests;
using System;

namespace Netension.Request
{
    /// <inheritdoc cref="IQuery{TResponse}"/>.
    public class Query<TResponse> : BaseRequest, IQuery<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TResponse}"/>.
        /// </summary>
        /// <param name="requestId">Unique id of the request. If it is null it will be generated.</param>
        public Query(Guid? requestId = null)
            : base(requestId ?? Guid.NewGuid())
        {
        }
    }
}
