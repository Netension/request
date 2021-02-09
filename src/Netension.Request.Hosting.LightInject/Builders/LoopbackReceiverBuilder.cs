using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Wrappers;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class LoopbackReceiverBuilder
    {
        public IHostBuilder Builder { get; }

        public LoopbackReceiverBuilder(IHostBuilder builder)
        {
            Builder = builder;
        }

        public void UseCorrelation()
        {
            Builder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.Decorate<ILoopbackRequestUnwrapper, LoopbackCorrelationUnwrapper>();
            });
        }
    }
}
