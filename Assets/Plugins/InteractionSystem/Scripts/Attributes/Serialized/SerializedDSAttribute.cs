using System;

namespace InteractionSystem
{
    public abstract class SerializedDSAttribute : DSAttribute
    {
        public abstract bool IsValidType(Type fieldType);
    }
}
