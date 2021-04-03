using Netension.Core;
using System.Collections.Generic;

namespace Netension.Request.NetCore.Asp.ValueObjects
{
    public class Error : ValueObject
    {
        public int Code { get; }
        public string Message { get; }

        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new object[] { Code, Message };
        }
    }

}
