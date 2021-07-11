using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Netension.Request.NetCore.Asp.Hosting.LightInject;

namespace Netension.Request.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return NetensionNetCoreHostBuilder.CreateDefaultHostBuilder<SampleWireup>(args);
        }
    }
}
