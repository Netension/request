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
    }
}
