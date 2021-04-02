using Netension.Core;

namespace Netension.Request.NetCore.Asp.Enumerations
{
    public class ErrorCodeEnumeration : Enumeration
    {
        public string Message { get; }

        public static ErrorCodeEnumeration ValidationFailed => new ErrorCodeEnumeration(200, "VALIDATION_FAILED", "Validation failed");
        public static ErrorCodeEnumeration VerificationError => new ErrorCodeEnumeration(300, "VERFICATION_ERROR", "Verification error");
        public static ErrorCodeEnumeration InternalServerError => new ErrorCodeEnumeration(100, "SERVER_ERROR", "Unexpected error");

        public ErrorCodeEnumeration(int id, string name, string message) 
            : base(id, name)
        {
            Message = message;
        }

    }
}
