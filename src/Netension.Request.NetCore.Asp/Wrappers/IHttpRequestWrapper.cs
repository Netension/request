using Netension.Request.Abstraction.Wrappers;
using System.Net.Http.Json;

namespace Netension.Request.NetCore.Asp.Wrappers
{
    public interface IHttpRequestWrapper : IRequestWrapper<JsonContent>
    {
    }
}
