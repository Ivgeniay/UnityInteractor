using System;

namespace InteractionSystem
{
    public abstract class DSAttribute : Attribute
    {
        public abstract bool IsValidType(Type fieldType);
    }
}
