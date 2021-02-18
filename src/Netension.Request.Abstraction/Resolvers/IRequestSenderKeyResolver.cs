using Netension.Request.Abstraction.Requests;
using System.Collections.Generic;

namespace Netension.Request.Abstraction.Resolvers
{
    public interface IRequestSenderKeyResolver
    {
        IEnumerable<string> Resolve(IRequest request);
    }
}
