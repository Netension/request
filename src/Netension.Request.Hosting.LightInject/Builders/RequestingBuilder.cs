﻿using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Extensions.Correlation;
using Netension.Request.Abstraction.Dispatchers;
using Netension.Request.Abstraction.Resolvers;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Containers;
using Netension.Request.Dispatchers;
using Netension.Request.Hosting.LightInject.Registrates;
using Netension.Request.Senders;
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

        public void RegistrateHandlers(Action<HandlerRegister> registering)
        {
            registering(new HandlerRegister(HostBuilder));
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
    }
}
