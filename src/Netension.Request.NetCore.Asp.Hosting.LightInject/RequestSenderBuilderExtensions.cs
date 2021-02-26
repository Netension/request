using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Builders;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Enumerations;
using Netension.Request.NetCore.Asp.Options;
using Netension.Request.NetCore.Asp.Senders;
using Netension.Request.NetCore.Asp.Wrappers;
using System;
using System.Net.Http;

namespace Netension.Request.Hosting.LightInject.Builders
{
    public static class RequestSenderBuilderExtensions
    {
        public static void RegistrateSender(this RequestSenderBuilder builder, HttpSenderEnumeration senderEnumeration)
        {
            builder.RegistrateHttpSender(senderEnumeration.Name, senderEnumeration.Configure, senderEnumeration.Build, senderEnumeration.Predicate);
        }

        public static void RegistrateHttpSender(this RequestSenderBuilder builder, string key, Action<HttpSenderOptions, IConfiguration> configure, Action<HttpSenderBuilder> build, Func<IRequest, bool> predicate)
        {
            builder.Register.Registrate(key, predicate);

            builder.HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddOptions<HttpSenderOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();

                services.AddHttpClient<HttpCommandSender>(key, (provider, client) => ConfigureClient(provider, client, key));
                services.AddHttpClient<HttpQuerySender>(key, (provider, client) => ConfigureClient(provider, client, key));
            });

            builder.HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IHttpRequestWrapper, HttpRequestWrapper>(key);

                container.RegisterScoped<ICommandSender>(factory => new HttpCommandSender(factory.GetInstance<IHttpClientFactory>().CreateClient(key), factory.GetInstance<IOptionsSnapshot<HttpSenderOptions>>().Get(key), factory.GetInstance<IHttpRequestWrapper>(key), factory.GetInstance<ILogger<HttpCommandSender>>()), key);
                container.RegisterScoped<IQuerySender>(factory => new HttpQuerySender(factory.GetInstance<IHttpClientFactory>().CreateClient(key), factory.GetInstance<IOptionsSnapshot<HttpSenderOptions>>().Get(key), factory.GetInstance<IHttpRequestWrapper>(key), factory.GetInstance<ILogger<HttpQuerySender>>()), key);
            });

            build(new HttpSenderBuilder(builder.HostBuilder, key));
        }

        private static void ConfigureClient(IServiceProvider provider, HttpClient client, string key)
        {
            var optionsSnapshot = provider.GetRequiredService<IOptionsSnapshot<HttpSenderOptions>>();
            var options = optionsSnapshot.Get(key);

            client.BaseAddress = options.BaseAddress;
        }
    }
}
