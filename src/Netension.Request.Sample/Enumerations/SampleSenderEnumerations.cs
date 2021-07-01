using Microsoft.Extensions.Configuration;
using Netension.Request.Hosting.LightInject.Enumerations;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Enumerations;

namespace Netension.Request.Sample.Enumerations
{
    public static class SampleSenders
    {
        public static LoopbackSenderEnumeration Loopback => new(0, (builder) => { builder.UseCorrelation(); }, (request) => true);
        public static HttpSenderEnumeration Http => new(1, (options, configuration) => configuration.GetSection("Self").Bind(options), (builder) => { builder.UseCorrelation(); }, (request) => false);
    }
}
