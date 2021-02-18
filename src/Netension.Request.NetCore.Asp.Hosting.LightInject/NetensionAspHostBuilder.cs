using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.Hosting.LightInject;
using Serilog;
using System;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject
{
    public static class NetensionAspHostBuilder
    {
        public static IHost Build<TConfiguration>(string[] args)
            where TConfiguration : IRequestingConfiguration, IAspHostConfiguration
        {
            var configuration = Activator.CreateInstance<TConfiguration>();

            return NetensionHostBuilder.Build<TConfiguration>(args)
                        .ConfigureWebHostDefaults(configuration.ConfigureWebHost)
                        .Build();
        }
    }
}
