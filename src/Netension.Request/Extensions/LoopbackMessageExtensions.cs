using Netension.Request.Defaults;
using System.Collections.Generic;

namespace Netension.Request.Messages
{
    public static class LoopbackMessageExtensions
    {
        public static void SetMessageType(this IDictionary<string, object> headers, string value)
        {
            headers.Add(RequestDefaults.Header.MessageType, value);
        }

        public static string GetMessageType(this IDictionary<string, object> headers)
        {
            if (!headers.TryGetValue(RequestDefaults.Header.MessageType, out object messageType)) return null;

            return (string)messageType;
        }
    }
}
