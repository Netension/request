using System;

namespace Netension.Request.Abstraction.Requests
{
    public interface IRequest: IEquatable<IRequest>
    {
        Guid RequestId { get; }
        string MessageType { get; }
    }
}
