using System;

namespace InteractionSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : GeneralDSAttribute
    {
        public string Description { get; set; }
        public DescriptionAttribute(string desc)
        {
            Description = desc;
        }
    }
}
