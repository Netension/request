using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Request.Hosting.LightInject;
using Netension.Request.Hosting.LightInject.Builders;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject
{
    public abstract class DefaultAspHostConfiguration : DefaultHostConfiguration, IAspHostConfiguration
    {
        public override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddControllers();
        }

        public override void ConfigureRequestReceivers(RequestReceiverBuilder builder)
        {
            builder.RegistrateHttpReceiver();
            builder.RegistrateLoopbackReceiver((LoopbackReceiverBuilder) => { });
        }

        public void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.Configure((app) =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapRequestReceiver();
                });
            });
        }
    }
}
