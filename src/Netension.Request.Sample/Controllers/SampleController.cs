﻿using Microsoft.AspNetCore.Mvc;
using Netension.Request.Abstraction.Senders;
using Netension.Request.Sample.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ICommandSender _commandSender;

        public SampleController(ICommandSender commandSender)
        {
            _commandSender = commandSender;
        }

        [HttpGet]
        public async Task Get()
        {
            await _commandSender.SendAsync(new SampleCommand(), CancellationToken.None);
        }
    }
}