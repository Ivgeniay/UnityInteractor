using System;

namespace InteractionSystem
{
    public sealed class EnumPropContextAttribute : SerializePropAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(Enum);
    }
}
