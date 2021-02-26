using Netension.Request.Abstraction.Wrappers;
using Netension.Request.Messages;

namespace Netension.Request.Wrappers
{
    /// <summary>
    /// Responsible for wrap <see cref="Abstraction.Requests.IRequest">IRequest</see> to <see cref="LoopbackMessage"/>.
    /// </summary>
    public interface ILoopbackRequestWrapper : IRequestWrapper<LoopbackMessage>
    {
    }
}
