using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Collections
{
    public class WellKnownEndpoint
    {
        public string Name { get; }
        public PathString Path { get; }

        public WellKnownEndpoint(string name, PathString path)
        {
            Name = name;
            Path = path;
        }
    }

    public class WellKnownEndpointsCollection
    {
        private readonly ICollection<WellKnownEndpoint> _endpoints = new List<WellKnownEndpoint>();

        public void AddEndpoint(string name, PathString path) => _endpoints.Add(new WellKnownEndpoint(name, path));

        public string AsHtml()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<strong>/.well-known</strong></br>");
            foreach (var endpoint in _endpoints)
                builder.AppendLine(AsHtml(endpoint));

            return builder.ToString();
        }

        private string AsHtml(WellKnownEndpoint endpoint)
        {
            var builder = new StringBuilder();
            builder.Append("<span style=\"margin: 16px\">- <a target=\"_blank\" href=\"").Append("/.well-known").Append(endpoint.Path).Append("\"/>").Append(endpoint.Name).AppendLine("</a></span></br>");

            return builder.ToString();
        }
    }
}
