using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Netension.Core;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Extensions;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Options;
using System;
using System.Drawing;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Enumerations
{
    public static class StringExtension
    {
        public static Color MapColor(this string status)
        {
            return Enum.Parse<HealthStatus>(status) switch
            {
                HealthStatus.Healthy => Color.Green,
                HealthStatus.Unhealthy => Color.Red,
                HealthStatus.Degraded => Color.Orange,
                _ => Color.Gray,
            };
        }
    }

    public class BadgeEnumeration : Enumeration
    {
        public string Path { get; }
        public Func<IServiceProvider, string> ValueFactory { get; }
        public Func<string, Color> ColorFactory { get; }

        public static BadgeEnumeration Version => new(0, "Version", "version", _ => ApplicationOptions.Version.ToString(), _ => Color.Blue);
        public static BadgeEnumeration Liveness => new(1, "Live", "live", provider => provider.GetRequiredService<HealthCheckService>().CheckLivenessAsync(default).GetAwaiter().GetResult().Status.ToString(), value => value.MapColor());
        public static BadgeEnumeration Readiness => new(2, "Ready", "ready", provider => provider.GetRequiredService<HealthCheckService>().CheckReadinessAsync(default).GetAwaiter().GetResult().Status.ToString(), value => value.MapColor());

        public BadgeEnumeration(int id, string name, string path, Func<IServiceProvider, string> valueFactory, Func<string, Color> colorFactory)
            : base(id, name)
        {
            Path = path;
            ValueFactory = valueFactory;
            ColorFactory = colorFactory;
        }
    }
}
