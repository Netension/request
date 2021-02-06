using Netension.Request.Abstraction.Requests;
using Netension.Request.Requests;
using System;

namespace Netension.Request
{
    public class Query<TResult> : BaseRequest, IQuery<TResult>
    {
        public Query(Guid? requestId = null) 
            : base(requestId)
        {
        }
    }
}
