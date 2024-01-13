using System;

namespace InteractionSystem
{
    public class FloatPropContextAttribute : SerializePropAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(float);
    }

}
