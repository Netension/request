using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Wrappers;
using System;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class LoopbackSenderBuilder
    {
        public IHostBuilder HostBuilder { get; }
        public string Key { get; }

        public LoopbackSenderBuilder(IHostBuilder hostBuilder, string key)
        {
            HostBuilder = hostBuilder;
            Key = key;
        }

        public void UseCorrelation()
        {
            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.Decorate(typeof(ILoopbackRequestWrapper), typeof(LoopbackCorrelationWrapper), (registration) => registration.ServiceName.Equals(Key, StringComparison.InvariantCultureIgnoreCase));
            });
        }
    }
}
