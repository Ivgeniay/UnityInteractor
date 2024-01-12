using System;

namespace InteractionSystem
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IntPropContextAttribute : DSAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(int);
    }
}
