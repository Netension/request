using Microsoft.AspNetCore.Http;
using Netension.Request.Abstraction.Wrappers;

namespace Netension.Request.NetCore.Asp.Unwrappers
{
    public interface IHttpRequestUnwrapper : IRequestUnwrapper<HttpRequest>
    {
    }
}
