using Netension.Extensions.Correlation.Defaults;
using Netension.Request.Abstraction.Defaults;
using System;
using System.Collections.Generic;

namespace Netension.Request.Extensions
{
    public static class HeadersExtensions
    {
        /// <summary>
        /// Add Message-Type header to <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="headers">Instance of the <see cref="IDictionary{TKey, TValue}"/>.</param>
        /// <param name="value">Value of the header.</param>
        public static void SetMessageType(this IDictionary<string, object> headers, string value)
        {
            headers.Add(RequestDefaults.Header.MessageType, value);
        }

        /// <summary>
        /// Get Message-Type header's value.
        /// </summary>
        /// <param name="headers">Instance of the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <returns>Value of the Message-Type header. If the Message-Type header does not present in the <see cref="IDictionary{TKey, TValue}"/> the result will be null.</returns>
        public static string GetMessageType(this IDictionary<string, object> headers)
        {
            if (!headers.TryGetValue(RequestDefaults.Header.MessageType, out object messageType)) return null;

            return (string)messageType;
        }

        /// <summary>
        /// Add Correlation-Id header to <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="headers">Instance of the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="value">Value of the Correlation-Id.</param>
        public static void SetCorrelationId(this IDictionary<string, object> headers, Guid value)
        {
            headers.Add(CorrelationDefaults.CorrelationId, value);
        }

        /// <summary>
        /// Get Correlation-Id header's value.
        /// </summary>
        /// <param name="headers">Instance of the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <returns>Value of the Correlation-Id header.</returns>
        /// <exception cref="InvalidOperationException">Throws if the Correlation-Id header does not present.</exception>
        public static Guid GetCorrelationId(this IDictionary<string, object> headers)
        {
            if (!headers.TryGetValue(CorrelationDefaults.CorrelationId, out object correlationId)) throw new InvalidOperationException($"{CorrelationDefaults.CorrelationId} header does not present");

            return (Guid)correlationId;
        }

        /// <summary>
        /// Add Causation-Id header to <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="headers">Instance of the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="value">Value of the Causation-Id.</param>
        public static void SetCausationId(this IDictionary<string, object> headers, Guid? value)
        {
            headers.Add(CorrelationDefaults.CausationId, value);
        }

        /// <summary>
        /// Get Causation-Id header's value.
        /// </summary>
        /// <param name="headers">Instance of the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <returns>Value of the Causation-ID header. If the Causation-Id header does not present in the <see cref="IDictionary{TKey, TValue}"/> the result will be null.</returns>
        public static Guid? GetCausationId(this IDictionary<string, object> headers)
        {
            if (!headers.TryGetValue(CorrelationDefaults.CausationId, out object causationId)) return null;

            return causationId as Guid?;
        }
    }
}
