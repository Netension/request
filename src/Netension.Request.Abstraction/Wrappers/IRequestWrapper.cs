using Netension.Request.Abstraction.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Abstraction.Wrappers
{
    /// <summary>
    /// Responsible for wrap the <see cref="IRequest"/>.
    /// </summary>
    /// <typeparam name="TEnvelop">Type of the envelop.</typeparam>
    public interface IRequestWrapper<TEnvelop>
    {
        /// <summary>
        /// Wrap the <see cref="IRequest"/> to <typeparamref name="TEnvelop"/>.
        /// </summary>
        /// <param name="request"><see cref="IRequest"/> instance.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <returns><typeparamref name="TEnvelop"/> instance with the wrapped <see cref="IRequest"/>.</returns>
        Task<TEnvelop> WrapAsync(IRequest request, CancellationToken cancellationToken);
    }
}
