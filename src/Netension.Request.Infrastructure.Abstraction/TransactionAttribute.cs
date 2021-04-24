using System;

namespace Netension.Request.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TransactionAttribute : Attribute
    {
        public string Database 
    }
}
