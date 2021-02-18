using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Resolvers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Senders;
using Netension.Request.Wrappers;
using System;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class RequestSenderBuilder
    {
        public IHostBuilder HostBuilder { get; }
        public IRequestSenderKeyRegister Register { get; }

        public RequestSenderBuilder(IHostBuilder hostBuilder, IRequestSenderKeyRegister register)
        {
            HostBuilder = hostBuilder;
            Register = register;
        }

        public void RegistrateLoopbackSender(Action<LoopbackSenderBuilder> build, Func<IRequest, bool> predicate)
        {
            RegistrateLoopbackSender(RequestingDefaults.Senders.LOOPBACK, build, predicate);
        }

        public void RegistrateLoopbackSender(string key, Action<LoopbackSenderBuilder> build, Func<IRequest, bool> predicate)
        {
            Register.Registrate(key, predicate);

            HostBuilder.ConfigureContainer<IServiceContainer>((container) =>
            {
                container.RegisterTransient<ICommandSender, LoopbackCommandSender>(key);
                container.RegisterTransient<IQuerySender, LoopbackQuerySender>(key);

                container.RegisterTransient<ILoopbackRequestWrapper, LoopbackRequestWrapper>(key);
            });

            build(new LoopbackSenderBuilder(HostBuilder, key));
        }
    }
}
