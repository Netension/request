using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Blazor.Senders;
using Netension.Request.Containers;
using Netension.Request.Http.Options;
using Netension.Request.Http.Senders;
using Netension.Request.Http.Wrappers;
using System;

namespace Netension.Request.Blazor.Hosting.LightInject.Builders
{
    public class RequestSenderRegisty
    {
        public WebAssemblyHostBuilder HostBuilder { get; }
        public RequestSenderKeyContainer Keys { get; }
        public IServiceContainer Container { get; }

        public RequestSenderRegisty(WebAssemblyHostBuilder hostBuilder, RequestSenderKeyContainer keys, IServiceContainer container)
        {
            HostBuilder = hostBuilder;
            Keys = keys;
            Container = container;
        }

        public RequestSenderRegisty RegistrateHttpSender(Action<HttpSenderOptions, IConfiguration> configure, Action<RequestSenderBuilder> build)
        {
            HostBuilder.Services.AddOptions<HttpSenderOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            HostBuilder.Services.AddScoped<IHttpRequestWrapper, HttpRequestWrapper>();

            HostBuilder.Services.AddHttpClient<ICommandSender, HttpCommandSender>((client, provider) =>
            {
                var options = provider.GetRequiredService<IOptions<HttpSenderOptions>>().Value;
                client.BaseAddress = options.BaseAddress;

                return new HttpCommandSender(client, provider.GetRequiredService<IOptions<HttpSenderOptions>>().Value, provider.GetRequiredService<IHttpRequestWrapper>(), provider.GetRequiredService<ILogger<HttpCommandSender>>());
            });

            HostBuilder.Services.AddHttpClient<IQuerySender, HttpQuerySender>((client, provider) =>
            {
                var options = provider.GetRequiredService<IOptions<HttpSenderOptions>>().Value;
                client.BaseAddress = options.BaseAddress;

                return new HttpQuerySender(client, provider.GetRequiredService<IOptions<HttpSenderOptions>>().Value, provider.GetRequiredService<IHttpRequestWrapper>(), provider.GetRequiredService<ILogger<HttpQuerySender>>());
            });

            build(new RequestSenderBuilder(HostBuilder, Container));

            Container.Decorate<IQuerySender, QueryExceptionHandlerMiddleware>();
            Container.Decorate<ICommandSender, CommandExceptionHandlerMiddleware>();

            return this;
        }
    }
}
