using Netension.Core;
using Netension.Request.Abstraction.Requests;
using System;

namespace Netension.Request.Hosting.LightInject.Enumerations
{
    public class SenderEnumeration<TBuilder> : Enumeration
    {
        public Action<TBuilder> Build { get; }
        public Func<IRequest, bool> Predicate { get; }

        public SenderEnumeration(int id, string name, Action<TBuilder> build, Func<IRequest, bool> predicate)
            : base(id, name)
        {
            Build = build;
            Predicate = predicate;
        }
    }
}
