using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Abstraction.Defaults;
using Netension.Request.Abstraction.Requests;
using System.Linq;

namespace System.Net.Http.Headers
{
    public static class HttpContentHeadersExtensions
    {
        public static string GetMessageType(this HttpContentHeaders headers)
        {
            return headers.GetValues(RequestDefaults.Header.MessageType).First();
        }

        public static void SetMessageType(this HttpContentHeaders headers, IRequest request)
        {
            headers.Add(RequestDefaults.Header.MessageType, request.MessageType);
        }   
        
        public static Guid GetCorrelationId(this HttpContentHeaders headers)
        {
            return Guid.Parse(headers.GetValues(CorrelationDefaults.CorrelationId).First());
        }

        public static void SetCorrelationId(this HttpContentHeaders headers, Guid correlationId)
        {
            headers.Add(CorrelationDefaults.CorrelationId, correlationId.ToString());
        }

        public static Guid GetCausationId(this HttpContentHeaders headers)
        {
            return Guid.Parse(headers.GetValues(CorrelationDefaults.CausationId).First());
        }

        public static void SetCausationId(this HttpContentHeaders headers, Guid causationId)
        {
            headers.Add(CorrelationDefaults.CausationId, causationId.ToString());
        }
    }
}
