using System; 

namespace NodeEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DSActorAttributes : Attribute
    {
        public int Order { get; set; }
        public string Description { get; set; }
    }
}
