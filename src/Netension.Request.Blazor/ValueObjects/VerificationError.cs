namespace Netension.Request.Blazor.ValueObjects
{
    public class VerificationError : Error
    {
        public VerificationError(int code, string message)
            : base(code, message)
        {
        }
    }
}
