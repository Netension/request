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
        private readonly IQuerySender _querySender;

        public SampleController(IQuerySender querySender)
        {
            _querySender = querySender;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return await _querySender.QueryAsync(new SampleQuery(), CancellationToken.None);
        }
    }
}
