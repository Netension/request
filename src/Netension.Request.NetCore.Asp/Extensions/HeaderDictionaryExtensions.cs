using Netension.Request.Abstraction.Defaults;
using System.Linq;

namespace Microsoft.AspNetCore.Http
{
    public static class HeaderDictionaryExtensions
    {
        public static string GetMessageType(this IHeaderDictionary headers)
        {
            if (!headers.ContainsKey(RequestDefaults.Header.MessageType)) return null;
            return headers[RequestDefaults.Header.MessageType].First();
        }
    }
}
