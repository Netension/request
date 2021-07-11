using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Netension.Request.NetCore.Asp.Hosting.LightInject.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Netension.Request.NetCore.Asp.Hosting.LightInject.Configurators
{
    public class SwaggerConfigurator : IConfigureOptions<SwaggerGeneratorOptions>
    {
        private readonly IOptions<ApplicationOptions> _applicationOptionsAccessor;

        public SwaggerConfigurator(IOptions<ApplicationOptions> applicationOptionsAccessor)
        {
            _applicationOptionsAccessor = applicationOptionsAccessor;
        }

        public void Configure(SwaggerGeneratorOptions options)
        {
            var applicationOptions = _applicationOptionsAccessor.Value;

            options.SwaggerDocs.Clear();
            options.SwaggerDocs.Add($"v{ApplicationOptions.Version.Major}", new OpenApiInfo
            {
                Title = $"{applicationOptions.Name} - {applicationOptions.Environment}",
                Description = applicationOptions.Description,
                Version = ApplicationOptions.Version.ToString(),
                Contact = new OpenApiContact
                {
                    Name = applicationOptions.Contact.Name,
                    Email = applicationOptions.Contact.Email,
                    Url = applicationOptions.Contact.Url
                }
            });
            options.SortKeySelector = description =>
            {
                var dictionary = new Dictionary<string, int>
                {
                    ["GET"] = 1,
                    ["POST"] = 2,
                    ["PUT"] = 3,
                    ["DELETE"] = 4
                };

                return dictionary[description.HttpMethod].ToString();
            };
        }
    }
}
