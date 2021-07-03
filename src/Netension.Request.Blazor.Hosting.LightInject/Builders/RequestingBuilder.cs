using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Netension.Extensions.Correlation;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Blazor.Brokers;
using Netension.Request.Blazor.Senders;
using Netension.Request.Containers;
using Netension.Request.Senders;
using System;

namespace Netension.Request.Blazor.Hosting.LightInject.Builders
{
    public class RequestingBuilder
    {
        public WebAssemblyHostBuilder HostBuilder { get; init; }

        public void RegistrateCorrelation()
        {
            HostBuilder.Services.RegistrateCorrelation();
        }

        public void UseErrorHandler()
        {
            HostBuilder.Services.AddSingleton<ErrorBroker>();
            HostBuilder.Services.AddSingleton<IErrorPublisher>(provider => provider.GetRequiredService<ErrorBroker>());
            HostBuilder.Services.AddSingleton<IErrorChannel>(provider => provider.GetRequiredService<ErrorBroker>());

            var container = new ServiceContainer();

            container.Decorate<ICommandSender, CommandExceptionHandlerMiddleware>();
            container.Decorate<IQuerySender, QueryExceptionHandlerMiddleware>();

            HostBuilder.ConfigureContainer(new LightInjectServiceProviderFactory(container));
        }

        public void RegistrateSenders(Action<RequestSenderRegisty> registrate)
        {
            var requestSenderKeyContainer = new RequestSenderKeyContainer();

            HostBuilder.Services.AddScoped<RequestSender>();

            registrate(new RequestSenderRegisty { HostBuilder = HostBuilder, Container = requestSenderKeyContainer });
        }
    }
}
