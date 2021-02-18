using LightInject;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Netension.Request.Hosting.LightInject
{
    public static class NetensionHostBuilder
    {
        public static IHostBuilder Build<TConfiguration>(string[] args)
            where TConfiguration : IRequestingConfiguration
        {
            var configuration = Activator.CreateInstance<TConfiguration>();

            return Host.CreateDefaultBuilder()
                        .UseSerilog((context, configuration) =>
                        {
                            configuration.ReadFrom.Configuration(context.Configuration);
                        })
                        .UseLightInject()
                        .ConfigureServices(configuration.ConfigureServices)
                        .ConfigureContainer<IServiceContainer>(configuration.ConfigureContainer)
                        .ConfigureRequesting(configuration);
        }
    }
}
