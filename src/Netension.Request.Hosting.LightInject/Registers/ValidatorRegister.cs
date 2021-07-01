using FluentValidation;
using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Hosting.LightInject.Extensions;
using System.Reflection;

namespace Netension.Request.Hosting.LightInject.Registers
{
    public class ValidatorRegister
    {
        public IHostBuilder HostBuilder { get; }

        public ValidatorRegister(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public void RegistrateValidator<TValidator, TCommand>()
            where TValidator : IValidator<TCommand>
            where TCommand : ICommand
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container => container.RegisterTransient<IValidator<TCommand>, TValidator>());
        }

        public void RegistrateValidator<TValidator, TQuery, TResponse>()
            where TValidator : IValidator<TQuery>
            where TQuery : IQuery<TResponse>
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container => container.RegisterTransient<IValidator<TQuery>, TValidator>());
        }

        public void RegistrateValidatorsFromAssembly(Assembly assembly)
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container => container.RegisterAssembly(assembly, (serviceType, implementingType) => implementingType.IsImplementGenericInterface(typeof(IValidator<>))));
        }

        public void RegistrateHandlerFromAssemblyOf<TType>()
        {
            RegistrateValidatorsFromAssembly(typeof(TType).Assembly);
        }
    }
}
