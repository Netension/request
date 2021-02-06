using Netension.Request.Abstraction.Requests;
using Netension.Request.Requests;
using System;

namespace Netension.Request
{
    public class Command : BaseRequest, ICommand
    {
        public Command(Guid? requestId = null) 
            : base(requestId)
        {
        }
    }
}
