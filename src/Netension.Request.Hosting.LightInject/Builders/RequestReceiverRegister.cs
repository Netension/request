using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Dispatchers;
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

        public void UseCorrelationLogger()
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.Decorate<ICommandDispatcher, CommandLoggingDispatcher>();
                container.Decorate<IQueryDispatcher, QueryLoggingDispatcher>();
            });
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
