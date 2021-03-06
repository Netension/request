﻿using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netension.Request.Blazor.Hosting.LightInject.Builders;
using Netension.Request.Blazor.Hosting.LightInject.Contexts;

namespace Netension.Request.Blazor.Hosting.LightInject
{
    public abstract class Wireup
    {
        protected internal abstract void ConfigureServices(HostContext context, IServiceCollection services);
        protected internal abstract void ConfigureContainer(HostContext context, IServiceRegistry registry);

        protected virtual internal void ConfigureRequesting(RequestingBuilder builder)
        {
            builder.RegistrateCorrelation();
            builder.UseErrorHandler();

            builder.RegistrateSenders(ConfigureSenders);
        }

        protected virtual void ConfigureSenders(RequestSenderRegisty registy)
        {
            registy.RegistrateHttpSender((options, configuration) => configuration.GetSection("Backend").Bind(options), builder => builder.UseCorrelation());
        }
    }
}
