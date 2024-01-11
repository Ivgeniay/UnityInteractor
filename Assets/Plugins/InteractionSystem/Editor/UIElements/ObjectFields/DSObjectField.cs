using System;
using UnityEditor.UIElements;

namespace NodeEngine.UIElement
{
    internal class DSObjectField : ObjectField
    {
        public string Name { get => label; set => label = value; } 
        public Type Type { get => objectType; set => objectType = value; } 
        public object Value { get => value; set => this.value = (UnityEngine.Object)value; }

        public DSObjectField()
        {
            Value = null;
        }
    }
}
