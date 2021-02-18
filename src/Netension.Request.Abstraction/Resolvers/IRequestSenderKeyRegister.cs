using Netension.Request.Abstraction.Requests;
using System;

namespace Netension.Request.Abstraction.Resolvers
{
    public interface IRequestSenderKeyRegister
    {
        void Registrate(string key, Func<IRequest, bool> predicate);
    }
}
