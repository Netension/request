using Netension.Request.Abstraction.Requests;
using System.Collections.Generic;

namespace Netension.Request.Messages
{
    public class LoopbackMessage
    {
        public IDictionary<string, object> Headers = new Dictionary<string, object>();
        public IRequest Request { get; }

        public LoopbackMessage(IRequest request)
        {
            Request = request;
        }

    }
}
