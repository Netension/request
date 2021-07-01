using System;
using System.Threading.Tasks;

namespace Netension.Request.Blazor.Handlers
{
    public interface IErrorHandler
    {
        Task HandleErrorAsync(Exception exception);
    }
}
