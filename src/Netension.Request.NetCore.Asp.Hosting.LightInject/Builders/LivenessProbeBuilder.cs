using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Defaults;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Builders
{
    public class LivenessProbeBuilder
    {
        public IHealthChecksBuilder Builder { get; }

        public LivenessProbeBuilder(IHealthChecksBuilder builder)
        {
            Builder = builder;
        }

        public LivenessProbeBuilder AddProbe(string name, Func<Task<HealthCheckResult>> check)
        {
            Builder.AddAsyncCheck(name, check, tags: new List<string> { HostingDefaults.HealthCheck.Tags.Liveness });
            return this;
        }

        public LivenessProbeBuilder AddSelfCheck()
        {
            return AddProbe("Self", () => Task.FromResult(HealthCheckResult.Healthy()));
        }
    }
}
