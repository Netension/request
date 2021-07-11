using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Configurators;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Defaults;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationInformation(this IServiceCollection services)
        {
            services.AddOptions<ApplicationOptions>()
                               .BindConfiguration(HostingDefaults.Configuration.ApplicationInformation)
                               .ValidateDataAnnotations();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddTransient<IConfigureOptions<SwaggerGeneratorOptions>, SwaggerConfigurator>();
            services.AddOptions<SwaggerUIOptions>()
                .Configure<IServiceProvider>((options, provider) =>
                {
                    var applicationOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>();
                    options.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{applicationOptions.Value.Name} v{ApplicationOptions.Version.Major}", Url = "/swagger/v1/swagger.json", } };
                });
        }
    }
}
