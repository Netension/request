using LightInject;
using Netension.Extensions.Correlation;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Dispatchers;
using Netension.Request.Hosting.LightInject.Builders;
using Netension.Request.Receivers;
using Netension.Request.Senders;
using Netension.Request.Wrappers;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class RequestExtensions
    {
        public static IHostBuilder RegistrateLoopbackSender(this IHostBuilder builder, Action<LoopbackSenderBuilder> build)
        {
            builder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<ILoopbackRequestWrapper, LoopbackRequestWrapper>();
                container.RegisterScoped<ICommandSender, LoopbackCommandSender>();
                container.RegisterScoped<IQuerySender, LoopbackQuerySender>();
            });

            build.Invoke(new LoopbackSenderBuilder(builder));

            return builder;
        }

        public static IHostBuilder RegistrateLoopbackReceiver(this IHostBuilder builder, Action<LoopbackReceiverBuilder> build)
        {
            builder.ConfigureContainer<IServiceContainer>((context, services) =>
            {
                services.RegisterScoped<ILoopbackRequestUnwrapper, LoopbackRequestUnwrapper>();

                services.RegisterScoped<ILoopbackRequestReceiver, LoopbackRequestReceiver>();
                services.Decorate<ILoopbackRequestReceiver, LoopbackScopeHandler>();

                services.RegisterScoped<ICommandDispatcher, CommandDispatcher>();
                services.RegisterScoped<IQueryDispatcher, QueryDispatcher>();
            });

            build.Invoke(new LoopbackReceiverBuilder(builder));

            return builder;
        }
    }
}
