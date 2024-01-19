using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace InteractionSystem
{
    internal class NodeContext
    {
        public readonly List<FieldInfo> FieldsSerAttribute = new List<FieldInfo>();
        public readonly List<PropertyInfo> PropSerAttribute = new List<PropertyInfo>();
        public readonly object[] GeneralAttributes;

        private object instance;
        public NodeContext(object instance) 
        {
            this.instance = instance;
            GeneralAttributes = instance
                                    .GetType()
                                    .GetCustomAttributes(typeof(GeneralDSAttribute), false);
        }

        public void Initialize()
        {
            Type[] attributeSubclasses = 
                Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DSAttribute)))
                .ToArray();

            var t = GetSerAttributed(instance);
            FieldsSerAttribute.AddRange(t.fields);
            PropSerAttribute.AddRange(t.property);
        }

        public object GetValue(FieldInfo field) => field.GetValue(instance); 
        public object GetValue(PropertyInfo prop) => prop.GetValue(instance); 
        public List<T> GetGeneral<T>(bool includeSubclass = false) where T : GeneralDSAttribute
        {
            Type type = typeof(T);
            List<T> list = GeneralAttributes.Where(e =>
            {
                Type eType = e.GetType();
                if (includeSubclass) {
                    if (eType == type || eType.IsSubclassOf(type)) return true;
                }
                else
                    if (eType == type) return true;
                
                return false;
            })
                .Cast<T>()
                .ToList();

            return list;
        }

        public void SetValue(FieldInfo field, object value) => field.SetValue(instance, value);
        public void SetValue(PropertyInfo prop, object value) => prop.SetValue(instance, value);
        
        private (FieldInfo[] fields, PropertyInfo[] property) GetSerAttributed(object instance)
        {
            Type type = instance.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(field => Attribute.IsDefined(field, typeof(SerializeFieldNode)))
                .ToArray();

            PropertyInfo[] property = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(SerializePropNode)))
                .ToArray();

            return (fields, property);
        }
    }
}
