using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Request.NetCore.Asp.Unwrappers;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Builders
{
    public class HttpReceiverBuilder
    {
        public IHostBuilder HostBuilder { get; }

        public HttpReceiverBuilder(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }


        public void UseCorrelation()
        {
            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.Decorate<IHttpRequestUnwrapper, HttpCorrelationUnwrapper>();
            });
        }
    }
}
