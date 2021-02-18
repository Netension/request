using Microsoft.AspNetCore.Mvc;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Sample.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IRequestSender _requestSender;

        public SampleController(IRequestSender requestSender)
        {
            _requestSender = requestSender;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return await _requestSender.QueryAsync(new SampleQuery(), CancellationToken.None);
        }
    }
}
