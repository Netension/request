﻿using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Wrappers
{
    internal class LoopbackRequestUnwrapper : ILoopbackRequestUnwrapper
    {
        private readonly ILogger<LoopbackRequestUnwrapper> _logger;

        public LoopbackRequestUnwrapper(ILogger<LoopbackRequestUnwrapper> logger)
        {
            _logger = logger;
        }

        public Task<IRequest> UnwrapAsync(LoopbackMessage envelop, CancellationToken cancellationToken)
        {
            return Task.FromResult(envelop.Request);
        }
    }
}
