using System;
using System.Linq;

namespace Netension.Request.Hosting.LightInject.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsImplementGenericInterface(this Type type, Type @interface)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(@interface));
        }
    }
}
