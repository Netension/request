using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Collections
{
    public class Badge
    {
        public string Name { get; }
        public string Path { get; }
        public Func<IServiceProvider, string> ValueFactory { get; }
        public Func<string, Color> ColorFactory { get; }

        public Badge(string name, string path, Func<IServiceProvider, string> valueFactory, Func<string, Color> colorFactory)
        {
            Name = name;
            Path = path;
            ValueFactory = valueFactory;
            ColorFactory = colorFactory;
        }
    }

    public class BadgeCollection
    {
        private readonly ICollection<Badge> _badges = new List<Badge>();

        public void Add(string name, string Path, Func<IServiceProvider, string> valueFactory, Func<string, Color> colorFactory) => _badges.Add(new Badge(name, Path, valueFactory, colorFactory));
        public Badge Get(string path) => _badges.SingleOrDefault(b => b.Path.Equals(path));
        public IEnumerable<Badge> Get() => _badges;
    }
}
