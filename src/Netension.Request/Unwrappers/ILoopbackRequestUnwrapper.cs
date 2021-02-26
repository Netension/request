using Netension.Request.Abstraction.Wrappers;
using Netension.Request.Messages;

namespace Netension.Request.Unwrappers
{
    /// <summary>
    /// Responsible for unwrap <see cref="Abstraction.Requests.IRequest">IRequest</see> from <see cref="LoopbackMessage"/>.
    /// </summary>
    public interface ILoopbackRequestUnwrapper : IRequestUnwrapper<LoopbackMessage>
    {
    }
}
