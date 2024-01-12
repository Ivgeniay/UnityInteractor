using System;

namespace InteractionSystem
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FloatFieldContextAttribute : DSAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(float);
    }
}
