﻿using LightInject;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Request.Hosting.LightInject.Builders;
using Netension.Request.Hosting.LightInject.Registers;
using Netension.Request.Hosting.LightInject.Registrates;
using Netension.Request.NetCore.Asp.Hosting.LightInject;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Builders;
using Netension.Request.Sample.Contexts;
using Netension.Request.Sample.Requests;
using System;

namespace Netension.Request.Sample
{
    public class SampleWireup : Wireup
    {
        public override void ConfigureContainer(IServiceRegistry registry)
        {
            registry.RegisterSingleton<SampleContext>();
        }

        public override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
        }

        public override void ConfigureReadinessProbe(ReadinessProbeBuilder builder)
        {
            builder.AddUrlProbe("Microsoft", new Uri("https://www.microsoft.com"));
            builder.AddUrlProbe(".NET", new Uri("https://dotnet.microsoft.com"));
            builder.AddUrlProbe("Blazor", new Uri("https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor"));
            builder.AddUrlProbe("Unhealthy", new Uri("https://unhealthy-readiness-probe.com"));
        }

        public override void RegisterRequestSenders(RequestSenderRegister register)
        {
            register.RegistrateLoopbackSender(builder => builder.UseCorrelation(), _ => true);
        }

        public override void RegistrateValidators(ValidatorRegister register)
        {
            register.RegistrateHandlerFromAssemblyOf<GetQuery>();
        }

        public override void RegistrateRequestReceivers(RequestReceiverRegister register)
        {
            register.RegistrateLoopbackRequestReceiver(builder => builder.UseCorrelation());
        }

        public override void RegistrateHandlers(HandlerRegister register)
        {
            register.RegistrateHandlerFromAssemblyOf<GetQuery>();
        }
    }
}
