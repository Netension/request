using Microsoft.Extensions.Hosting;
using Netension.Request.Infrastructure.EFCore.Builders;
using System;

namespace Netension.Request.Infrastructure.EFCore
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseEFCore(this IHostBuilder builder, Action<EFCoreBuilder> build)
        {
            build(new EFCoreBuilder(builder));
            return builder;
        }
    }
}
