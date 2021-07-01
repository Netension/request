using System;
using System.Runtime.Serialization;

namespace Netension.Request.Blazor.ValueObjects
{
    public class RequestException : Exception
    {
        public int Code { get; }

        public RequestException()
        {
        }

        public RequestException(string message) : base(message)
        {
        }

        public RequestException(int code, string message) : base(message) => Code = code;

        public RequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
