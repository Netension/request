using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Extensions.Correlation;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Resolvers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Containers;
using Netension.Request.Dispatchers;
using Netension.Request.Hosting.LightInject.Registers;
using Netension.Request.Hosting.LightInject.Registrates;
using Netension.Request.Senders;
using Netension.Request.Validators;
using System;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public class RequestingBuilder
    {
        public IHostBuilder HostBuilder { get; }

        public RequestingBuilder(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public void RegistrateHandlers<TType>()
        {
            RegistrateHandlers(register => register.RegistrateHandlerFromAssemblyOf<TType>());
        }

        public void RegistrateHandlers(Action<HandlerRegister> registrate)
        {
            registrate(new HandlerRegister(HostBuilder));
        }

        public void RegistrateCorrelation()
        {
            HostBuilder.ConfigureServices(services =>
            {
                services.RegistrateCorrelation();
            });
        }

        public void RegistrateRequestSenders(Action<RequestSenderRegister> build)
        {
            var requestSenderKeyContainer = new RequestSenderKeyContainer();

            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.RegisterScoped<RequestSender>();
                container.RegisterScoped<ICommandSender>(factory => factory.GetInstance<RequestSender>());
                container.RegisterScoped<IQuerySender>(factory => factory.GetInstance<RequestSender>());

                container.RegisterInstance<IRequestSenderKeyResolver>(requestSenderKeyContainer);
            });

            build(new RequestSenderRegister(HostBuilder, requestSenderKeyContainer));
        }

        public void RegistrateRequestReceivers(Action<RequestReceiverRegister> build)
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.RegisterScoped<ICommandDispatcher, CommandDispatcher>();
                container.RegisterScoped<IQueryDispatcher, QueryDispatcher>();
            });

            build(new RequestReceiverRegister(HostBuilder));
        }

        public void RegistrateValidators<TType>()
        {
            RegistrateValidators(register => register.RegistrateHandlerFromAssemblyOf<TType>());
        }

        public void RegistrateValidators(Action<ValidatorRegister> registrate)
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.Decorate(typeof(ICommandHandler<>), typeof(CommandValidator<>));
                container.Decorate(typeof(IQueryHandler<,>), typeof(QueryValidator<,>));
            });

            registrate(new ValidatorRegister(HostBuilder));
        }
    }
}
