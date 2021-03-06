﻿using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Hosting.LightInject.Extensions;
using System.Reflection;

namespace Netension.Request.Hosting.LightInject.Registrates
{
    public class HandlerRegister
    {
        private readonly IHostBuilder _hostBuilder;

        public HandlerRegister(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        public void RegistrateHandlerFromAssemblyOf<TType>()
        {
            RegistrateHandlersFromAssembly(Assembly.GetAssembly(typeof(TType)));
        }

        public void RegistrateHandlersFromAssembly(Assembly assembly)
        {
            _hostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.RegisterAssembly(assembly, () => new PerScopeLifetime(), (serviceType, implementingType) =>
                implementingType.IsImplementGenericInterface(typeof(ICommandHandler<>)) || implementingType.IsImplementGenericInterface(typeof(IQueryHandler<,>)));
            });
        }

        public void RegistrateHandler<TQuery, TResponse, THandler>()
            where TQuery : IQuery<TResponse>
            where THandler : IQueryHandler<TQuery, TResponse>
        {
            _hostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.RegisterScoped<IQueryHandler<TQuery, TResponse>, THandler>();
            });
        }

        public void RegistrateHandler<TCommand, THandler>()
            where TCommand : ICommand
            where THandler : ICommandHandler<TCommand>
        {
            _hostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.RegisterScoped<ICommandHandler<TCommand>, THandler>();
            });
        }
    }
}
