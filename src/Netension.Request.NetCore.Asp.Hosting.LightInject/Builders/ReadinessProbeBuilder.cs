using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Defaults;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Builders
{
    public class ReadinessProbeBuilder
    {
        public IHealthChecksBuilder Builder { get; }

        public ReadinessProbeBuilder(IHealthChecksBuilder builder) => Builder = builder;

        public ReadinessProbeBuilder AddProbe(string name, Func<Task<HealthCheckResult>> check)
        {
            Builder.AddAsyncCheck(name, check, new List<string> { HostingDefaults.HealthCheck.Tags.Readiness });
            return this;
        }

        public ReadinessProbeBuilder AddUrlProbe(string name, Func<IServiceProvider, Uri> provider)
        {
            Builder.AddUrlGroup(provider, name, tags: new List<string> { HostingDefaults.HealthCheck.Tags.Readiness });
            return this;
        }

        public ReadinessProbeBuilder AddUrlProbe(string name, Uri uri)
        {
            return AddUrlProbe(name, _ => uri);
        }
    }
}
