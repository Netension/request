using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netension.Request.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoopbackController : ControllerBase
    {
        [HttpGet]
        public void Get()
        {
            
        }
    }
}
