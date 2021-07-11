using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Collections;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Enumerations;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Registries;
using System;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseBadges(this IHostBuilder builder, Action<BadgeRegistry> registrate)
        {
            var collection = new BadgeCollection();
            registrate(new BadgeRegistry(collection));

            return builder.ConfigureContainer<IServiceRegistry>(registry => registry.RegisterInstance(collection));
        }
    }
}
