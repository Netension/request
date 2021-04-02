using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Abstraction.Defaults;
using System;
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

        public static Guid GetCorrelationId(this IHeaderDictionary headers)
        {
            if (!headers.ContainsKey(CorrelationDefaults.CorrelationId)) return Guid.NewGuid();
            return Guid.Parse(headers[CorrelationDefaults.CorrelationId].First());
        }

        public static Guid? GetCausationId(this IHeaderDictionary headers)
        {
            if (!headers.ContainsKey(CorrelationDefaults.CausationId)) return null;
            var value = headers[CorrelationDefaults.CausationId].First();
            if (string.IsNullOrEmpty(value)) return null;
            return Guid.Parse(value);
        }
    }
}
