using Netension.Request.Abstraction.Requests;
using System.Collections.Generic;

namespace Netension.Request.Messages
{
    /// <summary>
    /// Bearer for <see cref="IRequest"/> for sending via <see cref="Senders.LoopbackQuerySender">LoopbackQuerySender</see> or <see cref="Senders.LoopbackCommandSender">LoopbackCommandSender</see>.
    /// </summary>
    public class LoopbackMessage
    {
        /// <summary>
        /// Headers of the message. It can bearer additional informations about the <see cref="IRequest"/>.
        /// </summary>
        public IDictionary<string, object> Headers = new Dictionary<string, object>();

        /// <summary>
        /// <see cref="IRequest"/> instance.
        /// </summary>
        public IRequest Request { get; }

        /// <summary>
        /// Initializes a new instance <see cref="LoopbackMessage"/>.
        /// </summary>
        /// <param name="request"><see cref="IRequest"/> to be sent.</param>
        public LoopbackMessage(IRequest request)
        {
            Request = request;
        }

    }
}
