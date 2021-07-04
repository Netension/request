using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;

namespace Netension.Request.Blazor.Hosting.LightInject.Contexts
{
    public class HostContext
    {
        public IConfiguration Configuration { get; init; }
        public IWebAssemblyHostEnvironment Environment { get; init; }
    }
}
