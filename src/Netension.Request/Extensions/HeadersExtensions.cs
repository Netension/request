using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Defaults;
using System;
using System.Collections.Generic;

namespace Netension.Request.Messages
{
    public static class HeadersExtensions
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

        public static void SetCorrelationId(this IDictionary<string, object> headers, Guid value)
        {
            headers.Add(CorrelationDefaults.CorrelationId, value);
        }

        public static Guid? GetCorrelationId(this IDictionary<string, object> headers)
        {
            if (!headers.TryGetValue(CorrelationDefaults.CorrelationId, out object correlationId)) return null;

            return (Guid)correlationId;
        }

        public static void SetCausationId(this IDictionary<string, object> headers, Guid value)
        {
            headers.Add(CorrelationDefaults.CausationId, value);
        }

        public static Guid? GetCausationId(this IDictionary<string, object> headers)
        {
            if (!headers.TryGetValue(CorrelationDefaults.CausationId, out object causationId)) return null;

            return (Guid)causationId;
        }
    }
}
