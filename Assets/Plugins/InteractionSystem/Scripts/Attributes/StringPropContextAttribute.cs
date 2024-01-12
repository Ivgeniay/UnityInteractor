using System;

namespace InteractionSystem
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StringPropContextAttribute : DSAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(string);
    }
}
