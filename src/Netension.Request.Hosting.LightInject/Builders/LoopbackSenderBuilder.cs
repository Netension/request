using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Wrappers;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class LoopbackSenderBuilder
    {
        public IHostBuilder HostBuilder { get; }

        public LoopbackSenderBuilder(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public void UseCorrelation()
        {
            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.Decorate<ILoopbackRequestWrapper, LoopbackCorrelationWrapper>();
            });
        }
    }
}
