using Microsoft.AspNetCore.Builder;
using Netension.Request.NetCore.Asp.Middlewares;

namespace Netension
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseErrorHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
