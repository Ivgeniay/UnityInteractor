using System;

namespace InteractionSystem
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class SerializePropAttribute : SerializedDSAttribute
    {
    }
}
