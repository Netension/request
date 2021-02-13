using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Netension.Request.NetCore.Asp.Options;
using System;

namespace Microsoft.AspNetCore.Routing
{
    public static class EndPointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapRequestReceiver(this IEndpointRouteBuilder routeBuilder)
        {
            return routeBuilder.MapRequestReceiver("/api");
        }

        public static IEndpointConventionBuilder MapRequestReceiver(this IEndpointRouteBuilder routeBuilder, PathString pattern)
        {
            return routeBuilder.MapRequestReceiver((options) => options.Pattern = pattern);
        }

        public static IEndpointConventionBuilder MapRequestReceiver(this IEndpointRouteBuilder routeBuilder, Action<RequestReceiverControllerOptions> configure)
        {
            var options = new RequestReceiverControllerOptions();
            configure.Invoke(options);

            return routeBuilder.MapControllerRoute("request-receiver", pattern: options.Pattern, defaults: new { controller = "RequestReceiver", action = "Post" });
        }
    }
}
