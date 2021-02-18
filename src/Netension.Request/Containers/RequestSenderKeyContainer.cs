using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Netension.Request.Containers
{
    public class RequestSenderKeyContainer : IRequestSenderKeyResolver, IRequestSenderKeyRegister
    {
        private readonly Dictionary<string, Func<IRequest, bool>> _keys = new Dictionary<string, Func<IRequest, bool>>();

        public void Registrate(string key, Func<IRequest, bool> predicate)
        {
            _keys.Add(key, predicate);
        }

        public IEnumerable<string> Resolve(IRequest request)
        {
            return _keys.Where(k => k.Value(request)).Select(k => k.Key);
        }
    }
}
