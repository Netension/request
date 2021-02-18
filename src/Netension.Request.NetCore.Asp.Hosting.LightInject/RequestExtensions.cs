using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.NetCore.Asp.Options;
using Netension.Request.NetCore.Asp.Receivers;
using Netension.Request.NetCore.Asp.Senders;
using Netension.Request.NetCore.Asp.Unwrappers;
using Netension.Request.NetCore.Asp.Wrappers;
using System;
using System.Net.Http;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public static class RequestExtensions
    {
        public static void RegistrateHttpReceiver(this RequestReceiverBuilder builder)
        {
            builder.HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IHttpRequestUnwrapper, HttpRequestUnwrapper>();
                container.RegisterScoped<IHttpRequestReceiver, HttpRequestReceiver>();
            });
        }

        public static void RegistrateHttpSender(this RequestSenderBuilder builder, Action<HttpSenderOptions, IConfiguration> configure, string key, Func<IRequest, bool> predicate)
        {
            builder.Register.Registrate(key, predicate);

            builder.HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.Register<ICommandSender>((factory) => new HttpCommandSender(factory.GetInstance<IHttpClientFactory>().CreateClient(key), factory.GetInstance<IOptionsSnapshot<HttpSenderOptions>>().Get(key), factory.GetInstance<IHttpRequestWrapper>(), factory.GetInstance<ILogger<HttpCommandSender>>()), key, new PerScopeLifetime());
                container.Register<IQuerySender>((factory) => new HttpQuerySender(factory.GetInstance<IHttpClientFactory>().CreateClient(key), factory.GetInstance<IOptionsSnapshot<HttpSenderOptions>>().Get(key), factory.GetInstance<IHttpRequestWrapper>(), factory.GetInstance<ILogger<HttpQuerySender>>()), key, new PerScopeLifetime());
            });

            builder.HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddOptions<HttpSenderOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();

                services.AddHttpClient<ICommandSender, HttpCommandSender>(key, (provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptionsSnapshot<HttpSenderOptions>>().Get(key);
                    client.BaseAddress = options.BaseAddress;
                });
                services.AddHttpClient<IQuerySender, HttpQuerySender>(key, (provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptionsSnapshot<HttpSenderOptions>>().Get(key);
                    client.BaseAddress = options.BaseAddress;
                });
            });
        }
    }
}
