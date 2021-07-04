using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Netension.Request.Blazor.Hosting.LightInject.Builders;
using Serilog;
using System;

namespace Netension.Request.Blazor.Hosting.LightInject
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static WebAssemblyHostBuilder UseSerilog(this WebAssemblyHostBuilder builder, IConfiguration configuration)
        {
            builder.Logging.ClearProviders()
                .AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger());

            return builder;
        }

        public static WebAssemblyHostBuilder UseLightInject(this WebAssemblyHostBuilder builder, IServiceContainer container)
        {
            builder.ConfigureContainer(new LightInjectServiceProviderFactory(container));

            return builder;
        }

        public static WebAssemblyHostBuilder UseRequesting(this WebAssemblyHostBuilder hostBuilder, Action<RequestingBuilder> build, IServiceContainer container)
        {
            var builder = new RequestingBuilder(hostBuilder, container);
            build(builder);

            return hostBuilder;
        }
    }
}
