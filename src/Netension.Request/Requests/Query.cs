using Netension.Request.Abstraction.Requests;
using Netension.Request.Requests;
using System;

namespace Netension.Request
{
    public class Query<TResponse> : BaseRequest, IQuery<TResponse>
    {
        public Query(Guid? requestId = null) 
            : base(requestId)
        {
        }
    }
}
