using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Netension.Request.Containers
{
    /// <inheritdoc/>
    public class RequestSenderKeyContainer : IRequestSenderKeyResolver, IRequestSenderKeyRegister
    {
        private readonly Dictionary<string, Func<IRequest, bool>> _keys = new Dictionary<string, Func<IRequest, bool>>();

        public void Registrate(string key, Func<IRequest, bool> predicate) => _keys.Add(key, predicate);

        public string Resolve(IRequest request)
        {
            var key = _keys.FirstOrDefault(k => k.Value(request));
            if (key.Equals(default(KeyValuePair<string, Func<IRequest, bool>>))) return null;

            return key.Key;
        }
    }
}
