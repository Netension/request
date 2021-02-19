using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddRequestReceiverController(this IMvcBuilder builder)
        {
            return builder.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
        }
    }
}
