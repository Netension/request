using Netension.Request.Hosting.LightInject.Builders;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseRequesting(this IHostBuilder hostBuilder, Action<RequestingBuilder> build)
        {
            var builder = new RequestingBuilder(hostBuilder);
            build(builder);

            return hostBuilder;
        }
    }
}
