using Netension.Request.Abstraction.Wrappers;
using Netension.Request.Messages;
using System.Threading.Tasks;

namespace Netension.Request.Wrappers
{
    public interface ILoopbackRequestWrapper : IRequestWrapper<LoopbackMessage>
    {
    }
}
