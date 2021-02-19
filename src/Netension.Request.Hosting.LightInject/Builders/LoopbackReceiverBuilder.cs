using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Unwrappers;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class LoopbackReceiverBuilder
    {
        public IHostBuilder Builder { get; }

        public LoopbackReceiverBuilder(IHostBuilder builder)
        {
            Builder = builder;
        }

        public LoopbackReceiverBuilder UseCorrelation()
        {
            Builder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.Decorate<ILoopbackRequestUnwrapper, LoopbackCorrelationUnwrapper>();
            });

            return this;
        }
    }
}
