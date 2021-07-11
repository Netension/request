using LightInject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Builders;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Configurators;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Extensions;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Reflection;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject
{
    public static class NetensionNetCoreHostBuilder
    {
        public static IHostBuilder CreateDefaultHostBuilder<TWireup>(string[] args)
            where TWireup : Wireup
        {
            var wireup = Activator.CreateInstance<TWireup>();

            return Host.CreateDefaultBuilder(args)
                    .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
                    .UseLightInject()
                    .ConfigureServices(services =>
                    {
                        services.AddApplicationInformation();

                        services.AddSwagger();

                        services.AddAuthentication();
                        services.AddAuthorization();

                        services.AddMvcCore()
                            .AddApplicationPart(Assembly.GetEntryAssembly());
                        services.AddControllers();

                        var healthChecksBuilder = services.AddHealthChecks();
                        wireup.ConfigureLivenessProbe(new LivenessProbeBuilder(healthChecksBuilder));
                        wireup.ConfigureReadinessProbe(new ReadinessProbeBuilder(healthChecksBuilder));
                    })
                    .ConfigureServices(wireup.ConfigureServices)
                    .ConfigureContainer<IServiceRegistry>(wireup.ConfigureContainer)
                    .UseRequesting(wireup.ConfigureRequesting)
                    .ConfigureWebHostDefaults(builder =>
                    {
                        builder.Configure(wireup.ConfigureRequestPipeline);
                    });
        }
    }
}
