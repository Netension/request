using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Receivers;
using Netension.Request.Wrappers;
using System;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class RequestReceiverBuilder
    {
        public IHostBuilder HostBuilder { get; }

        public RequestReceiverBuilder(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public RequestReceiverBuilder RegistrateLoopbackReceiver(Action<LoopbackReceiverBuilder> build)
        {
            HostBuilder.ConfigureContainer<IServiceContainer>((context, services) =>
            {
                services.RegisterScoped<ILoopbackRequestUnwrapper, LoopbackRequestUnwrapper>();

                services.RegisterScoped<ILoopbackRequestReceiver, LoopbackRequestReceiver>();
                services.Decorate<ILoopbackRequestReceiver, LoopbackScopeHandler>();
            });

            build.Invoke(new LoopbackReceiverBuilder(HostBuilder));

            return this;
        }
    }
}
