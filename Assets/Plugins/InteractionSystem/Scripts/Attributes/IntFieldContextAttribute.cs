using System;

namespace InteractionSystem
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class IntFieldContextAttribute : DSAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(int);
    }
}
