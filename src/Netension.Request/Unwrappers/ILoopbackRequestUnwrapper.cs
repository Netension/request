using Netension.Request.Abstraction.Wrappers;
using Netension.Request.Messages;

namespace Netension.Request.Unwrappers
{
    public interface ILoopbackRequestUnwrapper : IRequestUnwrapper<LoopbackMessage>
    {
    }
}
