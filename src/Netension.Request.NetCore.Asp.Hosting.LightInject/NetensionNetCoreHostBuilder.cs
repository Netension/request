using LightInject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Configurators;
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
            where TWireup : DefaultWireup
        {
            var wireup = Activator.CreateInstance<TWireup>();

            return Host.CreateDefaultBuilder(args)
                    .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
                    .UseLightInject()
                    .ConfigureServices(services =>
                    {
                        services.AddOptions<ApplicationOptions>()
                            .BindConfiguration("Application")
                            .ValidateDataAnnotations();

                        services.AddSwaggerGen();
                        services.AddTransient<IConfigureOptions<SwaggerGeneratorOptions>, SwaggerConfigurator>();
                    services.AddOptions<SwaggerUIOptions>()
                        .Configure<IServiceProvider>((options, provider) =>
                        {
                            var applicationOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>();
                            options.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{applicationOptions.Value.Name} v{ApplicationOptions.Version.Major}", Url = "/swagger/v1/swagger.json", } };                            
                        });

                        services.AddAuthentication();
                        services.AddAuthorization();

                        services.AddMvcCore()
                            .AddApiExplorer()
                            .AddApplicationPart(Assembly.GetEntryAssembly());
                        services.AddControllers();
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
