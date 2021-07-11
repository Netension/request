using Microsoft.AspNetCore.Http;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Defaults
{
    public static class HostingDefaults
    {
        public static readonly PathString WellKnown = "/.well-known";

        public static class HealthCheck
        {
            public static class Tags
            {
                public const string Liveness = "Liveness";
                public const string Readiness = "Readiness";
            }
        }

        public static class Configuration
        {
            public const string ApplicationInformation = "Application";
        }
    }
}
