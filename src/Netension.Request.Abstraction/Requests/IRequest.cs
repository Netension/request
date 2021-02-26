using System;

namespace Netension.Request.Abstraction.Requests
{
    /// <summary>
    /// Base type of the requests. Equatable by RequestId property.
    /// </summary>
    public interface IRequest: IEquatable<IRequest>
    {
        /// <summary>
        /// Unique id of the request.
        /// </summary>
        /// <remarks>
        /// Requests are equals by RequestId.
        /// </remarks>
        Guid? RequestId { get; }
        /// <summary>
        /// Type of the message. 
        /// </summary>
        /// <remarks>
        /// It will be used for determine type of the incoming message. <br/>
        ///  
        /// Current value: {TypeFullName}, {Assembly without version}
        /// </remarks>
        string MessageType { get; }
    }
}
