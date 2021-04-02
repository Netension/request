using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Receivers;
using Netension.Request.Unwrappers;
using System;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class RequestReceiverRegister
    {
        public IHostBuilder HostBuilder { get; }

        public RequestReceiverRegister(IHostBuilder HostBuilder)
        {
            this.HostBuilder = HostBuilder;
        }

        public void RegistrateLoopbackRequestReceiver(Action<LoopbackReceiverBuilder> build)
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.RegisterTransient<ILoopbackRequestReceiver, LoopbackRequestReceiver>();
                container.Decorate<ILoopbackRequestReceiver, LoopbackScopeHandler>();

                container.RegisterTransient<ILoopbackRequestUnwrapper, LoopbackRequestUnwrapper>();
            });

            build(new LoopbackReceiverBuilder(HostBuilder));
        }
    }
}
