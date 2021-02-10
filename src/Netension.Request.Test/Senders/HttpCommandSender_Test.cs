using Microsoft.Extensions.Logging;
using Netension.Request.NetCore.Asp.Senders;
using Xunit.Abstractions;

namespace Netension.Request.Test.Senders
{
    public class HttpCommandSender_Test
    {
        private readonly ILogger<HttpCommandSender> _logger;

        public HttpCommandSender_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<HttpCommandSender>();
        }

        private HttpCommandSender CreateSUT()
        {
            return new HttpCommandSender();
        }
    }
}
