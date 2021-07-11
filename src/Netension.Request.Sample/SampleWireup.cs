using LightInject;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Request.Hosting.LightInject.Builders;
using Netension.Request.Hosting.LightInject.Registers;
using Netension.Request.Hosting.LightInject.Registrates;
using Netension.Request.NetCore.Asp.Hosting.LightInject;
using Netension.Request.Sample.Contexts;
using Netension.Request.Sample.Requests;

namespace Netension.Request.Sample
{
    public class SampleWireup : DefaultWireup
    {
        public override void ConfigureContainer(IServiceRegistry registry)
        {
            registry.RegisterSingleton<SampleContext>();
        }

        public override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {

        }

        public override void RegisterRequestSenders(RequestSenderRegister register)
        {
            register.RegistrateLoopbackSender(builder => builder.UseCorrelation(), _ => true);
        }

        public override void RegistrateHandlers(HandlerRegister register)
        {
            register.RegistrateHandlerFromAssemblyOf<GetQuery>();
        }

        public override void RegistrateRequestReceivers(RequestReceiverRegister register)
        {
            register.RegistrateLoopbackRequestReceiver(builder => builder.UseCorrelation());
        }

        public override void RegistrateValidators(ValidatorRegister register)
        {
            register.RegistrateHandlerFromAssemblyOf<GetQuery>();
        }
    }
}
