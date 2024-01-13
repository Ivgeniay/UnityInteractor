using System;

namespace InteractionSystem
{ 
    public class StringFieldContextAttribute : SerializeFieldAttribute
    {
        public override bool IsValidType(Type fieldType) 
            => fieldType == typeof(string); 
    }
}
