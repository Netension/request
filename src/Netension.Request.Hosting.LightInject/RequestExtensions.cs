using LightInject;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Dispatchers;
using Netension.Request.Receivers;
using Netension.Request.Senders;
using Netension.Request.Wrappers;

namespace Microsoft.Extensions.Hosting
{
    public static class RequestExtensions
    {
        public static IHostBuilder RegistrateLoopbackSender(this IHostBuilder builder)
        {
            return builder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<ILoopbackRequestWrapper, LoopbackRequestWrapper>();
                container.RegisterScoped<ICommandSender, LoopbackCommandSender>();
            });
        }

        public static IHostBuilder RegistrateLoopbackReceiver(this IHostBuilder builder)
        {
            return builder.ConfigureContainer<IServiceContainer>((context, services) =>
            {
                services.RegisterScoped<ILoopbackRequestUnwrapper, LoopbackRequestUnwrapper>();

                services.RegisterScoped<ILoopbackRequestReceiver, LoopbackRequestReceiver>();
                services.Decorate<ILoopbackRequestReceiver, LoopbackScopeHandler>();

                services.RegisterScoped<ICommandDispatcher, CommandDispatcher>();
                services.RegisterScoped<IQueryDispatcher, QueryDispatcher>();
            });
        }
    }
}
