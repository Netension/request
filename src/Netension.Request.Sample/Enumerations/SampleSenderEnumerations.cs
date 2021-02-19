using Microsoft.Extensions.Configuration;
using Netension.Request.Hosting.LightInject.Enumerations;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Enumerations;
using Netension.Request.Sample.Requests;

namespace Netension.Request.Sample.Enumerations
{
    public class SampleSenders
    {
        public static LoopbackSenderEnumeration Loopback => new LoopbackSenderEnumeration(0, (builder) => { builder.UseCorrelation(); }, (request) => request is SampleQuery);
        public static HttpSenderEnumeration Http => new HttpSenderEnumeration(1, (options, configuration) => configuration.GetSection("Self").Bind(options), (builder) => { builder.UseCorrelation(); }, (request) => request is SampleCommand);
    }
}
