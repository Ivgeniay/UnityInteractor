using System;

namespace InteractionSystem
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class SerializeFieldAttribute : SerializedDSAttribute
    {
    }
}
