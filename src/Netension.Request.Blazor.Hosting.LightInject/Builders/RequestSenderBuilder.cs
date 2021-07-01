using LightInject;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Netension.Request.Http.Wrappers;

namespace Netension.Request.Blazor.Hosting.LightInject.Builders
{
    public class RequestSenderBuilder
    {
        public WebAssemblyHostBuilder HostBuilder { get; }
        public IServiceContainer Container { get; }

        internal RequestSenderBuilder(WebAssemblyHostBuilder hostBuilder, IServiceContainer container)
        {
            HostBuilder = hostBuilder;
            Container = container;
        }

        public RequestSenderBuilder UseCorrelation()
        {
            Container.Decorate<IHttpRequestWrapper, HttpCorrelationWrapper>();

            return this;
        }
    }
}
