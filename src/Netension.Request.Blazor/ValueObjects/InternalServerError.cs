using Netension.Request.Http.Enumerations;

namespace Netension.Request.Blazor.ValueObjects
{
    public class InternalServerError : Error
    {
        public InternalServerError()
            : base(ErrorCodeEnumeration.InternalServerError.Id, ErrorCodeEnumeration.InternalServerError.Message)
        {
        }
    }
}
