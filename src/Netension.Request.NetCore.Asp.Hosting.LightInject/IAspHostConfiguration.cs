using Microsoft.AspNetCore.Hosting;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject
{
    public interface IAspHostConfiguration
    {
        void ConfigureWebHost(IWebHostBuilder builder);
    }
}
