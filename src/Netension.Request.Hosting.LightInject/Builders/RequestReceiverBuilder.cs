using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Receivers;
using Netension.Request.Unwrappers;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class RequestReceiverBuilder
    {
        public IHostBuilder HostBuilder { get; }

        public RequestReceiverBuilder(IHostBuilder HostBuilder)
        {
            this.HostBuilder = HostBuilder;
        }

        public void RegistrateLoopbackRequestReceiver()
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.RegisterTransient<ILoopbackRequestReceiver, LoopbackRequestReceiver>();
                container.Decorate<ILoopbackRequestReceiver, LoopbackScopeHandler>();

                container.RegisterTransient<ILoopbackRequestUnwrapper, LoopbackRequestUnwrapper>();
            });
        }
    }
}
