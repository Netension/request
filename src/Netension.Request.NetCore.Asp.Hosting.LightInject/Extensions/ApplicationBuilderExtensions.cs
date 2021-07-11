using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Defaults;
using System.Linq;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseLivenessProbe(this IApplicationBuilder builder)
        {
            builder.UseTaggedHealthCheckProbe($"{HostingDefaults.WellKnown}/live", HostingDefaults.HealthCheck.Tags.Liveness);
        }

        public static void UseReadinessProbe(this IApplicationBuilder builder)
        {
            builder.UseTaggedHealthCheckProbe($"{HostingDefaults.WellKnown}/ready", HostingDefaults.HealthCheck.Tags.Readiness);
        }

        public static void UseTaggedHealthCheckProbe(this IApplicationBuilder builder, PathString path, string tag)
        {
            builder.UseHealthChecks(path, new HealthCheckOptions
            {
                Predicate = registration => registration.Tags?.Any() != true || registration.Tags.Contains(tag)
            });
        }
    }
}
