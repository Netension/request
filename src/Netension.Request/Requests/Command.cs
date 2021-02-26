using Netension.Request.Abstraction.Requests;
using Netension.Request.Requests;
using System;

namespace Netension.Request
{
    /// <inheritdoc cref="ICommand"/>
    public class Command : BaseRequest, ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/>.
        /// </summary>
        /// <param name="requestId">Unique id of the request. If it is null it will be generated.</param>
        public Command(Guid? requestId = null)
            : base(requestId ?? Guid.NewGuid())
        {
        }
    }
}
