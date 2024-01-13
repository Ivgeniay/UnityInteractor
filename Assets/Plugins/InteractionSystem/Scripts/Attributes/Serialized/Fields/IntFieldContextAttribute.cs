using System;

namespace InteractionSystem
{ 
    public class IntFieldContextAttribute : SerializeFieldAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(int);
    }
}
