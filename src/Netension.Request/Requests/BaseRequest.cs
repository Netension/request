using Netension.Request.Abstraction.Requests;
using System;

namespace Netension.Request.Requests
{
    /// <inheritdoc cref="IRequest"/>
    public class BaseRequest : IRequest
    {
        public Guid? RequestId { get; }
        public string MessageType => $"{GetType().FullName}, {GetType().Assembly.GetName().Name}";

        protected BaseRequest(Guid? requestId) => RequestId = requestId;

        /// <summary>
        /// Server hash code for the <see cref="IRequest"/> based on the RequestId.
        /// </summary>
        /// <returns>Generated hash code.</returns>
        public override int GetHashCode() => -2107324841 + RequestId.GetHashCode();

        public override bool Equals(object obj) => Equals(this, obj as IRequest);

        public bool Equals(IRequest x, IRequest y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.RequestId.Equals(y.RequestId)) return true;
            return false;
        }

        public int GetHashCode(IRequest obj) => obj.GetHashCode();
    }
}
