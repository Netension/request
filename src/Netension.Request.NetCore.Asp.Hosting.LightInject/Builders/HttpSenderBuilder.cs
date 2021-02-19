using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.NetCore.Asp.Wrappers;
using System;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Builders
{
    public class HttpSenderBuilder
    {
        public IHostBuilder HostBuilder { get; }
        public string Key { get; }

        public HttpSenderBuilder(IHostBuilder hostBuilder, string key)
        {
            HostBuilder = hostBuilder;
            Key = key;
        }

        public HttpSenderBuilder UseCorrelation()
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.Decorate(typeof(IHttpRequestWrapper), typeof(HttpCorrelationWrapper), (registration) => registration.ServiceName.Equals(Key, StringComparison.InvariantCultureIgnoreCase));
            });

            return this;
        }
    }
}
