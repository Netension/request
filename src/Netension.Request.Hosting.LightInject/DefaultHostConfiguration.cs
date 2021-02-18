using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Request.Hosting.LightInject.Builders;

namespace Netension.Request.Hosting.LightInject
{
    public abstract class DefaultHostConfiguration : IRequestingConfiguration
    {
        public virtual void ConfigureContainer<IServiceContainer>(HostBuilderContext context, IServiceContainer container)
        {
            
        }

        public virtual void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            
        }

        public abstract void ConfigureRequestSenders(RequestSenderBuilder builder);
        public abstract void ConfigureRequestReceivers(RequestReceiverBuilder builder);
    }
}
