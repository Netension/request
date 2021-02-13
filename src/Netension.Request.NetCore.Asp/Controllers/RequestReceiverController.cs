using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netension.Request.NetCore.Asp.Receivers;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.ConnectionHandlers
{
    public class RequestReceiverController : ControllerBase
    {
        private readonly IHttpRequestReceiver _requestReceiver;
        private readonly IHttpContextAccessor _contextAccessor;

        public RequestReceiverController(IHttpRequestReceiver requestReceiver, IHttpContextAccessor contextAccessor)
        {
            _requestReceiver = requestReceiver;
            _contextAccessor = contextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveAsync(_contextAccessor.HttpContext.Request, cancellationToken);
        }
    }
}
