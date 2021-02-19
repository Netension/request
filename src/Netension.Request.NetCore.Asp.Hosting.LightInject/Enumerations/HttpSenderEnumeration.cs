using Microsoft.Extensions.Configuration;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Hosting.LightInject.Enumerations;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Builders;
using Netension.Request.NetCore.Asp.Options;
using System;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Enumerations
{
    public class HttpSenderEnumeration : SenderEnumeration<HttpSenderBuilder>
    {
        public Action<HttpSenderOptions, IConfiguration> Configure { get; }

        public HttpSenderEnumeration(int id, Action<HttpSenderOptions, IConfiguration> configure, Action<HttpSenderBuilder> build, Func<IRequest, bool> predicate)
            : this(id, "http", configure, build, predicate)
        {
        }

        public HttpSenderEnumeration(int id, string name, Action<HttpSenderOptions, IConfiguration> configure, Action<HttpSenderBuilder> build, Func<IRequest, bool> predicate) 
            : base(id, name, build, predicate)
        {
            Configure = configure;
        }
    }
}
