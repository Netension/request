using Netension.Request.Abstraction.Receivers;
using Netension.Request.Messages;

namespace Netension.Request.Receivers
{
    /// <summary>
    /// Receive message from Loopback channel.
    /// </summary>
    /// <remarks>
    /// It will unwrap the message from <see cref="LoopbackMessage"/> envelop/>.
    /// </remarks>
    public interface ILoopbackRequestReceiver : IRequestReceiver<LoopbackMessage, object>
    {
    }
}
