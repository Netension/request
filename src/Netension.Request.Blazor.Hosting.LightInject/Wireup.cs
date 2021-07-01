using LightInject;
using Microsoft.Extensions.DependencyInjection;
using Netension.Request.Blazor.Hosting.LightInject.Contexts;

namespace Netension.Request.Blazor.Hosting.LightInject
{
    public abstract class Wireup
    {
        protected internal abstract void ConfigureServices(HostContext context, IServiceCollection services);
        protected internal abstract void ConfigureContainer(HostContext context, IServiceRegistry registry);
    }
}
