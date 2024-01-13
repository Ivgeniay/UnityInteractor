using System;

namespace InteractionSystem
{ 
    public class StringPropContextAttribute : SerializePropAttribute
    {
        public override bool IsValidType(Type fieldType)
            => fieldType == typeof(string);
    }
}
