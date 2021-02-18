using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Abstraction.Defaults;
using System.Linq;

namespace System.Net.Http.Headers
{
    public static class HttpContentHeaderExtensions
    {
        public static string GetMessageType(this HttpContentHeaders headers)
        {
            return headers.GetValues(RequestDefaults.Header.MessageType).First();
        }

        public static Guid GetCorrelationId(this HttpContentHeaders headers)
        {
            return Guid.Parse(headers.GetValues(CorrelationDefaults.CorrelationId).First());
        }

        public static Guid GetCausationId(this HttpContentHeaders headers)
        {
            return Guid.Parse(headers.GetValues(CorrelationDefaults.CausationId).First());
        }
    }
}
