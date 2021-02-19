using Netension.Request.Abstraction.Requests;
using Netension.Request.Hosting.LightInject.Builders;
using System;

namespace Netension.Request.Hosting.LightInject.Enumerations
{
    public class LoopbackSenderEnumeration : SenderEnumeration<LoopbackSenderBuilder>
    {
        public LoopbackSenderEnumeration(int id, Action<LoopbackSenderBuilder> build, Func<IRequest, bool> predicate)
            : this(id, "loopback", build, predicate)
        {
        }

        public LoopbackSenderEnumeration(int id, string name, Action<LoopbackSenderBuilder> build, Func<IRequest, bool> predicate) 
            : base(id, name, build, predicate)
        {
        }
    }
}
