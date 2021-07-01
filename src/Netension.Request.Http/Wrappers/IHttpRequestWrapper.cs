using Netension.Request.Abstraction.Wrappers;
using System.Net.Http.Json;

namespace Netension.Request.Http.Wrappers
{
    public interface IHttpRequestWrapper : IRequestWrapper<JsonContent>
    {
    }
}
