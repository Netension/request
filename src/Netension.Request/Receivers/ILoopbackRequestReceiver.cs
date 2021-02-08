using Netension.Request.Abstraction.Receivers;
using Netension.Request.Messages;

namespace Netension.Request.Receivers
{
    public interface ILoopbackRequestReceiver : IRequestReceiver<LoopbackMessage, object>
    {
    }
}
