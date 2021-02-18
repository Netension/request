using LightInject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
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
                .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
                .UseLightInject()
                .UseRequesting(builder =>
                {
                    builder.HostBuilder.ConfigureContainer<IServiceContainer>(container =>
                    {
                        container.RegisterScoped<IQueryHandler<SampleQuery, string>, SampleQueryHandler>();
                    });
                    builder.RegistrateRequestSenders(builder =>
                    {
                        builder.RegistrateLoopbackSender(builder => { }, request => true);
                    });
                    builder.RegistrateRequestReceivers(builder =>
                    {
                        builder.RegistrateLoopbackRequestReceiver();
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
