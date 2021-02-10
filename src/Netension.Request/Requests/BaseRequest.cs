using Netension.Request.Abstraction.Requests;
using System;

namespace Netension.Request.Requests
{
    public  class BaseRequest : IRequest
    {
        public Guid? RequestId { get; }
        public string MessageType => $"{GetType().FullName}, {GetType().Assembly.GetName().Name}";

        protected BaseRequest(Guid? requestId)
        {
            RequestId = requestId;
        }

        public bool Equals(IRequest other)
        {
            return other != null && RequestId.Equals(other.RequestId);
        }

        public override int GetHashCode()
        {
            return -2107324841 + RequestId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IRequest);
        }
    }
}
