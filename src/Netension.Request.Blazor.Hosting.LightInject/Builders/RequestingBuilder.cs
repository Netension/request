using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Netension.Extensions.Correlation;
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

        public void RegistrateSenders(Action<RequestSenderRegisty> registrate)
        {
            var requestSenderKeyContainer = new RequestSenderKeyContainer();

            HostBuilder.Services.AddScoped<RequestSender>();

            registrate(new RequestSenderRegisty { HostBuilder = HostBuilder, Container = requestSenderKeyContainer });
        }
    }
}
