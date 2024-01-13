using System;

namespace InteractionSystem
{
    public class BoolFieldContextAttribute : SerializeFieldAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(bool);
    }
}
