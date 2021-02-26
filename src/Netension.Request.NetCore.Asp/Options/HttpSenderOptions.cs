using System;
using System.ComponentModel.DataAnnotations;

namespace Netension.Request.NetCore.Asp.Options
{
    public class HttpSenderOptions
    {
        [Required]
        public Uri BaseAddress { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
