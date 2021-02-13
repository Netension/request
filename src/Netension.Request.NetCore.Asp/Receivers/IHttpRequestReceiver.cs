using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netension.Request.Abstraction.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Receivers
{
    public interface IHttpRequestReceiver : IRequestReceiver<HttpRequest, IActionResult>
    {
    }
}
