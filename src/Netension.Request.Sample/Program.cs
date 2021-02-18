using Microsoft.Extensions.Hosting;
using Netension.Request.Hosting.LightInject;
using Netension.Request.NetCore.Asp.Hosting.LightInject;

namespace Netension.Request.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NetensionAspHostBuilder.Build<HostConfiguration>(args).Run();
        }
    }
}
