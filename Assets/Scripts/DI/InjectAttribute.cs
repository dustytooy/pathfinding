using System;

namespace Dustytoy.DI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
    }
}
