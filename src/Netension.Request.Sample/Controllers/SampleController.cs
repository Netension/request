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
        private readonly ICommandSender _commandSender;
        private readonly IQuerySender _querySender;

        public SampleController(ICommandSender commandSender, IQuerySender querySender)
        {
            _commandSender = commandSender;
            _querySender = querySender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            return Ok(await _querySender.QueryAsync(new GetQuery(), cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("{value}")]
        public async Task<IActionResult> GetAsync(string value, CancellationToken cancellationToken)
        {
            return Ok(await _querySender.QueryAsync(new GetDetailQuery(value), cancellationToken).ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]CreateCommand command, CancellationToken cancellationToken)
        {
            await _commandSender.SendAsync(command, cancellationToken).ConfigureAwait(false);
            return Accepted();
        }

        [HttpPut("{value}")]
        public async Task<IActionResult> UpdateAsync(string originalValue, [FromBody]string newValue, CancellationToken cancellationToken)
        {
            await _commandSender.SendAsync(new UpdateCommand(originalValue, newValue), cancellationToken).ConfigureAwait(false);
            return Accepted();
        }

        [HttpDelete("{value}")]
        public async Task<IActionResult> DeleteAsync(string value, CancellationToken cancellationToken)
        {
            await _commandSender.SendAsync(new DeleteCommand(value), cancellationToken).ConfigureAwait(false);
            return Accepted();
        }
    }
}
