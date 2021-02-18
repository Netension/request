using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Request.Hosting.LightInject.Builders;

namespace Netension.Request.Hosting.LightInject
{
    public interface IRequestingConfiguration
    {
        void ConfigureServices(HostBuilderContext context, IServiceCollection services);
        void ConfigureContainer<IServiceContainer>(HostBuilderContext context, IServiceContainer container);
        void ConfigureRequestSenders(RequestSenderBuilder builder);
        void ConfigureRequestReceivers(RequestReceiverBuilder builder);
    }
}
