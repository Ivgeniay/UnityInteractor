using System;

namespace InteractionSystem
{
    public sealed class EnumFieldContextAttribute : SerializeFieldAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(Enum);
    }
}
