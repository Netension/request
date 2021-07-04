using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Netension.Request.Blazor.Hosting.LightInject.Contexts;
using System;

namespace Netension.Request.Blazor.Hosting.LightInject
{
    public static class NetensionWebAssemblyHostBuilder
    {
        public static WebAssemblyHostBuilder CreateDefaultHostBuilder<TWireup, TApp>(string[] args)
            where TWireup : Wireup
            where TApp : IComponent
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            var wireup = Activator.CreateInstance<TWireup>();

            builder.RootComponents.Add<TApp>("#app");

            var configuration = builder.Configuration.Build();
            var container = new ServiceContainer(ContainerOptions.Default.WithMicrosoftSettings());

            builder.UseSerilog(configuration);

            var context = new HostContext { Configuration = configuration, Environment = builder.HostEnvironment };

            wireup.ConfigureServices(context, builder.Services);
            wireup.ConfigureContainer(context, container);

            builder.UseLightInject(container);

            builder.UseRequesting(wireup.ConfigureRequesting, container);

            return builder;
        }
    }
}
