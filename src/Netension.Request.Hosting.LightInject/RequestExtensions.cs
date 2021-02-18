using LightInject;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Resolvers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Containers;
using Netension.Request.Dispatchers;
using Netension.Request.Hosting.LightInject;
using Netension.Request.Hosting.LightInject.Builders;
using Netension.Request.Senders;

namespace Microsoft.Extensions.Hosting
{
    public static class RequestExtensions
    {
        public static IHostBuilder ConfigureRequesting(this IHostBuilder builder, IRequestingConfiguration configuration)
        {
            var requestSenderKeyContainer = new RequestSenderKeyContainer();

            var requestSenderBuilder = new RequestSenderBuilder(builder, requestSenderKeyContainer);
            var requestReceiverBuilder = new RequestReceiverBuilder(builder);

            configuration.ConfigureRequestSenders(requestSenderBuilder);
            configuration.ConfigureRequestReceivers(requestReceiverBuilder);

            return builder.ConfigureContainer<IServiceContainer>((context, services) =>
            {
                services.RegisterScoped<ICommandDispatcher, CommandDispatcher>();
                services.RegisterScoped<IQueryDispatcher, QueryDispatcher>();
                services.RegisterTransient<IRequestSender, RequestSender>();

                services.RegisterInstance(requestSenderKeyContainer);
                services.RegisterSingleton<IRequestSenderKeyRegister>((factory) => factory.GetInstance<RequestSenderKeyContainer>());
                services.RegisterSingleton<IRequestSenderKeyResolver>((factory) => factory.GetInstance<RequestSenderKeyContainer>());
            });
        }
    }
}
