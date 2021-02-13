using LightInject;
using Netension.Request.NetCore.Asp.Receivers;
using Netension.Request.NetCore.Asp.Unwrappers;

namespace Microsoft.Extensions.Hosting
{
    public static class RequestExtensions
    {
        public static IHostBuilder RegistrateHttpRequestReceiver(this IHostBuilder builder)
        {
            return builder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IHttpRequestUnwrapper, HttpRequestUnwrapper>();
                container.RegisterScoped<IHttpRequestReceiver, HttpRequestReceiver>();
            });
        }
    }
}
