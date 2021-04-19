using LightInject;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Annotations;
using Netension.Request.Hosting.LightInject.Builders;
using Netension.Request.Infrastructure.EFCore.Handlers;

namespace Netension.Request.Infrastructure.EFCore
{
    public static class RequestingBuilderExtensions
    {
        public static void RegistrateTransactionHandlers(this RequestingBuilder builder)
        {
            builder.HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.Decorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandler<>), registration => registration.ImplementingType?.GetCustomAttributes(typeof(TransactionAttribute), true).Length > 0);
                container.Decorate(typeof(IQueryHandler<,>), typeof(TransactionQueryHandler<,>), registration => registration.ImplementingType?.GetCustomAttributes(typeof(TransactionAttribute), true).Length > 0);
            });
        }
    }
}
