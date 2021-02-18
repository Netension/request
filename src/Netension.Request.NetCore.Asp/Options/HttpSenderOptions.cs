using Microsoft.AspNetCore.Http;
using System;

namespace Netension.Request.NetCore.Asp.Options
{
    public class HttpSenderOptions
    {
        public Uri BaseAddress { get; set; }
        public PathString Path { get; set; }
    }
}
