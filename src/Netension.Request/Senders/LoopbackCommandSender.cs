﻿using Microsoft.Extensions.Logging;
using Netension.Request.Abstraction.Requests;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Wrappers;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Senders
{
    public class LoopbackCommandSender : ICommandSender
    {
        private readonly ILoopbackRequestWrapper _wrapper;
        private readonly ILogger<LoopbackCommandSender> _logger;

        public LoopbackCommandSender(ILoopbackRequestWrapper wrapper, ILogger<LoopbackCommandSender> logger)
        {
            _wrapper = wrapper;
            _logger = logger;
        }

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            _logger.LogDebug("Send {id} command via {type} sender", command.RequestId, "loopback");

            await _wrapper.WrapAsync(command, cancellationToken);
        }
    }
}
