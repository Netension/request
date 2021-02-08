using Netension.Request.Abstraction.Wrappers;
using Netension.Request.Messages;

namespace Netension.Request.Wrappers
{
    public interface ILoopbackRequestUnwrapper : IRequestUnwrapper<LoopbackMessage>
    {
    }
}
