using Microsoft.Extensions.Configuration;
using Netension.Request.Hosting.LightInject.Builders;
using Netension.Request.NetCore.Asp.Hosting.LightInject;
using Netension.Request.Sample.Requests;

namespace Netension.Request.Sample
{
    public class HostConfiguration : DefaultAspHostConfiguration
    {
        public override void ConfigureRequestSenders(RequestSenderBuilder builder)
        {
            builder.RegistrateLoopbackSender((loopbackSenderBuilder) => { }, "loopback", (request) => request is SampleQuery);
            builder.RegistrateHttpSender((options, configuration) => configuration.GetSection("Self").Bind(options), "http", (request) => request is SampleCommand);
        }
    }
}
