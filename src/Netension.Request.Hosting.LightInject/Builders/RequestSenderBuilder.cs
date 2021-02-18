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

        public RequestSenderBuilder RegistrateLoopbackSender(Action<LoopbackSenderBuilder> build, string key, Func<IRequest, bool> predicate)
        {
            Register.Registrate(key, predicate);

            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<ILoopbackRequestWrapper, LoopbackRequestWrapper>(key);
                container.RegisterScoped<ICommandSender, LoopbackCommandSender>(key);
                container.RegisterScoped<IQuerySender, LoopbackQuerySender>(key);
            });

            build.Invoke(new LoopbackSenderBuilder(HostBuilder, key));

            return this;
        }
    }
}
