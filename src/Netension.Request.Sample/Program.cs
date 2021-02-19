using LightInject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Netension.Request.Abstraction.Handlers;
using Netension.Request.NetCore.Asp.Hosting.LightInject;
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
                .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
                .UseLightInject()
                .UseRequesting(builder =>
                {
                    builder.RegistrateCorrelation();

                    builder.RegistrateHandlers((register) =>
                    {
                        register.RegistrateHandlerFromAssemblyOf<Startup>();
                    });

                    builder.RegistrateRequestSenders(builder =>
                    {
                        builder.RegistrateLoopbackSender(builder => { builder.UseCorrelation(); }, request => true);
                        builder.RegistrateHttpSender("http", (options, configuration) => configuration.GetSection("Self").Bind(options), (builder) => { }, (request) => request is SampleCommand);
                    });

                    builder.RegistrateRequestReceivers(builder =>
                    {
                        builder.RegistrateLoopbackRequestReceiver(builder => { builder.UseCorrelation(); });
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
