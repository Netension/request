using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Wrappers;

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

        public LoopbackSenderBuilder UseCorrelation()
        {
            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.Decorate<ILoopbackRequestWrapper, LoopbackCorrelationWrapper>();
            });

            return this;
        }
    }
}
