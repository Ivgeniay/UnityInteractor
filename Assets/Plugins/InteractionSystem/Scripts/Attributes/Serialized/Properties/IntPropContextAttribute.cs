using System;

namespace InteractionSystem
{
    public class IntPropContextAttribute : SerializePropAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(int);
    }
}
