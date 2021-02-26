using Netension.Request.Abstraction.Requests;
using System.Collections.Generic;

namespace Netension.Request.Abstraction.Resolvers
{
    /// <summary>
    /// Resolve the proper key according to <seealso cref="IRequestSenderKeyRegister">registrations</seealso>.
    /// </summary>
    /// <remarks>
    /// If the registered predicate (via <see cref="IRequestSenderKeyRegister"/>) true, the key will be resolved.
    /// </remarks>
    public interface IRequestSenderKeyResolver
    {
        /// <summary>
        /// Resolve key's according to the <see cref="IRequest"/>.
        /// </summary>
        /// <param name="request"><see cref="IRequest"/> to be sent.</param>
        /// <returns>List of the resolved keys.</returns>
        IEnumerable<string> Resolve(IRequest request);
    }
}
