using Netension.Core;

namespace Netension.Request.Http.Enumerations
{
    public class ErrorCodeEnumeration : Enumeration
    {
        public string Message { get; }

        public static ErrorCodeEnumeration ValidationFailed => new(200, "VALIDATION_FAILED", "Validation failed");
        public static ErrorCodeEnumeration VerificationError => new(300, "VERFICATION_ERROR", "Verification error");
        public static ErrorCodeEnumeration InternalServerError => new(100, "SERVER_ERROR", "Unexpected error");
        public static ErrorCodeEnumeration NotFound => new(404, "NOT_FOUND", "Requested resource was not found");
        public static ErrorCodeEnumeration Unathorized => new(401, "UNATHORIZED", "The user has to login to perform this operation");
        public static ErrorCodeEnumeration Forbidden => new(403, "FORBIDDEN", "The user does not have permission to perform this operation");
        public static ErrorCodeEnumeration Conflict => new(309, "CONFLICT", "The given resource has already exist");

        public ErrorCodeEnumeration(int id, string name, string message)
            : base(id, name)
        {
            Message = message;
        }
    }
}
