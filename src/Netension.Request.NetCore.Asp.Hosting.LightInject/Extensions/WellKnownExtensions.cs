using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Collections;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Defaults;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Options;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Extensions
{
    public static class WellKnownExtensions
    {
        public static void MapVersion(this IEndpointRouteBuilder builder)
        {
            var wellKnownEndpoints = builder.ServiceProvider.GetRequiredService<WellKnownEndpointsCollection>();
            wellKnownEndpoints.AddEndpoint("Version", "/version");

            builder.MapGet($"{HostingDefaults.WellKnown}/version", context =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                context.Response.WriteAsync(ApplicationOptions.Version.ToString(), context.RequestAborted);

                return Task.CompletedTask;
            });
        }

        public static void MapWellKnown(this IEndpointRouteBuilder builder)
        {
            builder.MapGet(HostingDefaults.WellKnown, context =>
            {
                var applicationOptions = context.RequestServices.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                var wellKnownCollection = context.RequestServices.GetRequiredService<WellKnownEndpointsCollection>();
                var healthCheckService = context.RequestServices.GetRequiredService<HealthCheckService>();

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Text.Html;
                var builder = new StringBuilder();
                builder.AppendLine("<html>");
                builder.AppendLine("<body>");
                builder.AppendLine(applicationOptions.AsHtml());
                builder.AppendLine(wellKnownCollection.AsHtml());
                builder.AppendLine("<strong>Health check:</strong></br>");
                builder.AppendLine("<strong style=\"margin: 16px\">Liveness probe:</strong></br>");
                builder.AppendLine(healthCheckService.CheckLivenessAsync(context.RequestAborted).GetAwaiter().GetResult().AsHtml());
                builder.AppendLine("<strong style=\"margin: 16px\">Readiness probe:</strong></br>");
                builder.AppendLine(healthCheckService.CheckReadinessAsync(context.RequestAborted).GetAwaiter().GetResult().AsHtml());
                builder.AppendLine("</body>");
                builder.AppendLine("</html>");

                context.Response.WriteAsync(builder.ToString());

                return Task.CompletedTask;
            });
        }

        public static string AsHtml(this ApplicationOptions options)
        {
            var builder = new StringBuilder();
            builder.Append("<strong>Name: </strong>").Append(options.Name).AppendLine("</br>");
            builder.Append("<strong>Environment: </strong>").Append(options.Environment).AppendLine("</br>");
            builder.Append("<strong>Description: </strong>").Append(options.Description).AppendLine("</br>");
            builder.Append("<strong>Version: </strong>").Append(ApplicationOptions.Version).AppendLine("</br>");
            builder.AppendLine("<strong>Contact:</strong></br>");
            builder.Append("<strong style=\"margin: 16px\">Name: </strong>").Append(options.Contact.Name).AppendLine("</span></br>");
            builder.Append("<strong style=\"margin: 16px\">Email: </strong><a href=\"mailto: ").Append(options.Contact.Email).Append("\">").Append(options.Contact.Email).AppendLine("</a></span></br>");
            builder.Append("<strong style=\"margin: 16px\">Url: </strong><a target=\"_blank\" href=\"").Append(options.Contact.Url).Append("\">").Append(options.Contact.Url).AppendLine("</a></span></br>");

            return builder.ToString();
        }

        public static async Task<HealthReport> CheckLivenessAsync(this HealthCheckService service, CancellationToken cancellationToken)
        {
            return await service.CheckHealthAsync(registration => registration.Tags?.Any() != true || registration.Tags.Contains(HostingDefaults.HealthCheck.Tags.Liveness), cancellationToken).ConfigureAwait(false);
        }

        public static async Task<HealthReport> CheckReadinessAsync(this HealthCheckService service, CancellationToken cancellationToken)
        {
            return await service.CheckHealthAsync(registration => registration.Tags?.Any() != true || registration.Tags.Contains(HostingDefaults.HealthCheck.Tags.Readiness), cancellationToken).ConfigureAwait(false);
        }

        public static string AsHtml(this HealthReport report)
        {
            var builder = new StringBuilder();
            foreach (var entry in report.Entries)
                builder.Append("<span style=\"margin: 32px\">").Append(entry.Key).Append(": ").Append(entry.Value.Status).AppendLine("</span></br>");

            return builder.ToString();
        }
    }
}
