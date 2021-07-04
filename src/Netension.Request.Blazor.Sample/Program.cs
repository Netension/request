using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Netension.Request.Blazor.Hosting.LightInject;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateWebHostBuilder(args).Build().RunAsync().ConfigureAwait(false);
        }

        public static WebAssemblyHostBuilder CreateWebHostBuilder(string[] args)
        {
            return NetensionWebAssemblyHostBuilder.CreateDefaultHostBuilder<SampleWireup, App>(args);
        }
    }
}
