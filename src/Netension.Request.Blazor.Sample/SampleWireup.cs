﻿using LightInject;
using Microsoft.Extensions.DependencyInjection;
using Netension.Request.Blazor.Hosting.LightInject;
using Netension.Request.Blazor.Hosting.LightInject.Builders;
using Netension.Request.Blazor.Hosting.LightInject.Contexts;

namespace Netension.Request.Blazor.Sample
{
    public class SampleWireup : Wireup
    {
        protected override void ConfigureContainer(HostContext context, IServiceRegistry registry)
        {

        }

        protected override void ConfigureServices(HostContext context, IServiceCollection services)
        {
            
        }

        protected override void ConfigureRequesting(RequestingBuilder builder)
        {
            base.ConfigureRequesting(builder);

            builder.UseErrorHandler<SampleErrorHandler>();
        }
    }
}
