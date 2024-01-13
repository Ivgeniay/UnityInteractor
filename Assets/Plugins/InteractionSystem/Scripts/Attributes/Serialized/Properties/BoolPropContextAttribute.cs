using System;

namespace InteractionSystem
{
    internal class BoolPropContextAttribute : SerializePropAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(int);
    }
}
