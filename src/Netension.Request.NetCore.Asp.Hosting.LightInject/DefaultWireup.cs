using LightInject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Request.Hosting.LightInject.Builders;
using Netension.Request.Hosting.LightInject.Registers;
using Netension.Request.Hosting.LightInject.Registrates;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject
{
    public abstract class DefaultWireup
    {
        public virtual void ConfigureRequesting(RequestingBuilder builder)
        {
            builder.RegistrateCorrelation();
            builder.RegistrateHandlers(RegistrateHandlers);
            builder.RegistrateValidators(RegistrateValidators);

            builder.RegistrateRequestReceivers(RegistrateRequestReceivers);
            builder.RegistrateRequestSenders(RegisterRequestSenders);
        }

        public virtual void ConfigureRequestPipeline(WebHostBuilderContext context, IApplicationBuilder builder)
        {
            builder.UseErrorHandling();

            if (context.HostingEnvironment.IsDevelopment())
            {
                builder.UseSwagger();
                builder.UseSwaggerUI();
            }

            builder.UseRouting();

            builder.UseAuthentication();
            builder.UseAuthorization();

            builder.UseEndpoints(ConfigureEndpoints);
        }

        public virtual void ConfigureEndpoints(IEndpointRouteBuilder builder)
        {
            builder.MapControllers();
            ConfigureRequestReceiver(builder.MapRequestReceiver());
        }

        public virtual void ConfigureRequestReceiver(IEndpointConventionBuilder builder)
        {

        }

        public abstract void RegistrateHandlers(HandlerRegister register);
        public abstract void RegistrateValidators(ValidatorRegister register);
        public abstract void ConfigureServices(HostBuilderContext context, IServiceCollection services);
        public abstract void ConfigureContainer(IServiceRegistry registry);
        public abstract void RegistrateRequestReceivers(RequestReceiverRegister register);
        public abstract void RegisterRequestSenders(RequestSenderRegister register);
    }
}
