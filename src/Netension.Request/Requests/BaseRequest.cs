using Netension.Request.Abstraction.Requests;
using System;

namespace Netension.Request.Requests
{
    /// <inheritdoc cref="IRequest"/>
    public  class BaseRequest : IRequest
    {
        public Guid? RequestId { get; }
        public string MessageType => $"{GetType().FullName}, {GetType().Assembly.GetName().Name}";

        protected BaseRequest(Guid? requestId)
        {
            RequestId = requestId;
        }

        /// <summary>
        /// True if the id of the requests are equal, otherwise false.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IRequest other)
        {
            return other != null && RequestId.Equals(other.RequestId);
        }

        /// <summary>
        /// Server hash code for the <see cref="IRequest"/> based on the RequestId.
        /// </summary>
        /// <returns>Generated hash code.</returns>
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
