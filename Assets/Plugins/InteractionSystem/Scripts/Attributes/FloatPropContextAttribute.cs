using System;

namespace InteractionSystem
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FloatPropContextAttribute : DSAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(float);
    }

}
