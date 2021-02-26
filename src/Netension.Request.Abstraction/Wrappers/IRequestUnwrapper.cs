using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Wrappers
{
    /// <summary>
    /// Responsible for unwrap <see cref="IRequest"/>.
    /// </summary>
    /// <typeparam name="TEnvelop">Type of the envelop.</typeparam>
    public interface IRequestUnwrapper<TEnvelop>
    {
        /// <summary>
        /// Unwrap the incoming <see cref="IRequest"/>.
        /// </summary>
        /// <param name="envelop">Incoming envelop.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <returns>Unwrapped <see cref="IRequest"/>.</returns>
        Task<IRequest> UnwrapAsync(TEnvelop envelop, CancellationToken cancellationToken);
    }
}
