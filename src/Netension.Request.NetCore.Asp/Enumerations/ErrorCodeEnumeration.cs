using Netension.Core;

namespace Netension.Request.NetCore.Asp.Enumerations
{
    public class ErrorCodeEnumeration : Enumeration
    {
        public string Message { get; }

        public static ErrorCodeEnumeration ValidationFailed => new(200, "VALIDATION_FAILED", "Validation failed");
        public static ErrorCodeEnumeration VerificationError => new(300, "VERFICATION_ERROR", "Verification error");
        public static ErrorCodeEnumeration InternalServerError => new(100, "SERVER_ERROR", "Unexpected error");

        public ErrorCodeEnumeration(int id, string name, string message) 
            : base(id, name)
        {
            Message = message;
        }
    }
}
