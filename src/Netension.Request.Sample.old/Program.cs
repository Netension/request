using LightInject;
using LightInject.Microsoft.AspNetCore.Hosting;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.Sample.Requests;
using Serilog;

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
                .ConfigureContainer<IServiceContainer>((container) =>
                {
                    container.RegisterScoped<ICommandHandler<SampleCommand>, SampleCommandHandler>();
                })
                .ConfigureServices((services) =>
                {
                    services.AddControllers();
                    services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" });
                    });
                })
                .RegistrateLoopbackSender()
                .RegistrateLoopbackReceiver()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure((app) =>
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));

                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
    }
}