using System;

namespace Dustytoy.DI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
    }
}
