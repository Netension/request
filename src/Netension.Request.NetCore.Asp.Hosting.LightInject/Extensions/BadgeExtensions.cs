using DotBadge;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Collections;
using System;
using System.Net.Mime;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Extensions
{
    public static class BadgeExtensions
    {
        public static void MapBadges(this IEndpointRouteBuilder builder)
        {
            builder.MapBadges("/.badge");
        }

        public static void MapBadges(this IEndpointRouteBuilder builder, string path)
        {
            builder.MapGet($"{path}/{{badge}}", async context =>
            {
                var path = (string)context.Request.RouteValues["badge"];
                var collection = context.RequestServices.GetRequiredService<BadgeCollection>();

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "image/svg+xml";
                await context.Response.WriteAsync(collection.Get(path).AsContent(context.RequestServices), context.RequestAborted).ConfigureAwait(false);
            });
        }

        public static string AsContent(this Badge badge, IServiceProvider provider)
        {
            var painter = new BadgePainter();

            var value = badge.ValueFactory(provider);
            return painter.DrawSVG(badge.Name, value, badge.ColorFactory(value).Name, Style.Plastic);
        }
    }
}
