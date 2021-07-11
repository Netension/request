using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Options
{
    public class ApplicationOptions
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Environment { get; set; }
        public string Description { get; set; }
        [Required]
        public ContactInformation Contact { get; set; }

        public static Version Version => new(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);

        public class ContactInformation
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Email { get; set; }
            public Uri Url { get; set; }
        }
    }
}
