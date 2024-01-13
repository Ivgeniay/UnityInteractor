using System;

namespace InteractionSystem
{ 
    public class FloatFieldContextAttribute : SerializeFieldAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(float);
    }
}
