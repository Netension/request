using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.Abstraction.Requests;
using System.Linq;

namespace System.Net.Http.Headers
{
    public static class HttpContentHeadersExtensions
    {
        public static void SetMessageType(this HttpContentHeaders headers, IRequest request)
        {
            headers.Add(RequestDefaults.Header.MessageType, request.MessageType);
        }  

        public static void SetCorrelationId(this HttpContentHeaders headers, Guid correlationId)
        {
            headers.Add(CorrelationDefaults.CorrelationId, correlationId.ToString());
        }

        public static void SetCausationId(this HttpContentHeaders headers, Guid causationId)
        {
            headers.Add(CorrelationDefaults.CausationId, causationId.ToString());
        }
    }
}
