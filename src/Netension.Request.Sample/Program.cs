using LightInject;
using LightInject.Microsoft.AspNetCore.Hosting;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Netension.Request.Sample.Requests;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.NetCore.Asp.Receivers;

namespace Netension.Request.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration);
                })
                .UseServiceProviderFactory((context) =>
                {
                    var options = ContainerOptions.Default.WithMicrosoftSettings().WithAspNetCoreSettings();
                    options.EnableCurrentScope = true;
                    return new LightInjectServiceProviderFactory(new ServiceContainer(options));
                })
                .ConfigureContainer<IServiceContainer>((context, container) =>
                {
                    container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
                    container.RegisterSingleton<IServiceFactory>((factory) => container);
                })
                .RegistrateCorrelation()
                .RegistrateLoopbackSender((builder) =>
                {
                    builder.UseCorrelation();
                })
                .RegistrateLoopbackReceiver((builder) =>
                {
                    builder.UseCorrelation();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure((app) =>
                    {
                        app.UseDeveloperExceptionPage();
                        app.UseSwagger();
                        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Netension.Request.Sample v1"));

                        app.UseRouting();

                        app.UseAuthorization();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapControllerRoute("generic", pattern: "/api", defaults: new { controller = "Generic", action = "Post" });
                        });
                    });
                })
                .ConfigureServices((services) =>
                {
                    services.AddControllers();
                    services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Netension.Request.Sample", Version = "v1" });
                    });

                    services.AddScoped<ICommandHandler<SampleCommand>, SampleCommandHandler>();
                    services.AddScoped<IQueryHandler<SampleQuery, string>, SampleQueryHandler>();
                });
    }
}
