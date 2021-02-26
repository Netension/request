using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netension.Request.Abstraction.Receivers;

namespace Netension.Request.NetCore.Asp.Receivers
{
    public interface IHttpRequestReceiver : IRequestReceiver<HttpRequest, IActionResult>
    {
    }
}
