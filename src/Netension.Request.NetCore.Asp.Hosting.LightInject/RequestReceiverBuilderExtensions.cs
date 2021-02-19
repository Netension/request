using LightInject;
using Microsoft.Extensions.DependencyInjection;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Builders;
using Netension.Request.NetCore.Asp.Receivers;
using Netension.Request.NetCore.Asp.Unwrappers;
using System;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public static class RequestReceiverBuilderExtensions
    {
        public static void RegistrateHttpRequestReceiver(this RequestReceiverBuilder builder, Action<HttpReceiverBuilder> build)
        {
            builder.HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddHttpContextAccessor();
            });

            builder.HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IHttpRequestReceiver, HttpRequestReceiver>();
                container.RegisterScoped<IHttpRequestUnwrapper, HttpRequestUnwrapper>();
            });

            build(new HttpReceiverBuilder(builder.HostBuilder));
        }
    }
}
