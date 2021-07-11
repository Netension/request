using Netension.Core;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Collections;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Enumerations;
using System;
using System.Drawing;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Registries
{
    public class BadgeRegistry
    {
        public BadgeCollection Collection { get; }

        public BadgeRegistry(BadgeCollection collection)
        {
            Collection = collection;
        }

        public void Registrate(string name, string path, Func<IServiceProvider, string> valueFactory, Func<string, Color> colorFactory) => Collection.Add(name, path, valueFactory, colorFactory);
        public void RegistrateVersion() => Registrate(BadgeEnumeration.Version.Name, BadgeEnumeration.Version.Path, BadgeEnumeration.Version.ValueFactory, BadgeEnumeration.Version.ColorFactory);
        public void RegistrateLiveness() => Registrate(BadgeEnumeration.Liveness.Name, BadgeEnumeration.Liveness.Path, BadgeEnumeration.Liveness.ValueFactory, BadgeEnumeration.Liveness.ColorFactory);
        public void RegistrateReadiness() => Registrate(BadgeEnumeration.Readiness.Name, BadgeEnumeration.Readiness.Path, BadgeEnumeration.Readiness.ValueFactory, BadgeEnumeration.Readiness.ColorFactory);
        public void RegistrateFrom<TEnumeration>()
            where TEnumeration : BadgeEnumeration
        {
            foreach (var badge in Enumeration.GetAll<TEnumeration>())
                Registrate(badge.Name, badge.Path, badge.ValueFactory, badge.ColorFactory);
        }
    }
}
