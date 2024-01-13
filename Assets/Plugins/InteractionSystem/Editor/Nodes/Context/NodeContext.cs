using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace InteractionSystem
{
    internal class NodeContext
    {
        public readonly Dictionary<Type, FieldInfo[]> FieldsAttributed = new();
        public readonly Dictionary<Type, PropertyInfo[]> PropertyAttributed = new();
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

            foreach (Type type in attributeSubclasses)
            {
                if (!type.IsSubclassOf(typeof(SerializedDSAttribute))) continue;
                
                FieldInfo[] res = GetAttributedFields(type);
                if (res.Length > 0)
                    FieldsAttributed[type] = res;
            }

            foreach (Type type in attributeSubclasses)
            {
                if (!type.IsSubclassOf(typeof(SerializedDSAttribute))) continue;

                PropertyInfo[] res = GetAttributedProperties(type);
                if (res.Length > 0)
                    PropertyAttributed[type] = res;
            }
             
        }


        public FieldInfo[] GetFields(Type attributeType)
        {
            if (FieldsAttributed.TryGetValue(attributeType, out FieldInfo[] value)) return value;
            return new FieldInfo[0];
        }
        public PropertyInfo[] GetProperties(Type attributeType)
        {
            if (PropertyAttributed.TryGetValue(attributeType, out PropertyInfo[] value)) return value;
            return new PropertyInfo[0];
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

        private FieldInfo[] GetAttributedFields(Type attributes)
        {
            Type type = instance.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, attributes))
                .ToArray();
            
            return fields.Where(e =>
            {
                DSAttribute dsAttribute = (DSAttribute)Attribute.GetCustomAttribute(e, typeof(DSAttribute));
                if (dsAttribute is SerializedDSAttribute s)
                    return s.IsValidType(e.FieldType);
                return true;
            }).ToArray();
        }
        private PropertyInfo[] GetAttributedProperties(Type attributes)
        {
            Type type = instance.GetType();
            PropertyInfo[] fields = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, attributes))
                .ToArray();

            return fields.Where(e =>
            {
                DSAttribute dsAttribute = (DSAttribute)Attribute.GetCustomAttribute(e, typeof(DSAttribute));
                if (dsAttribute is SerializedDSAttribute s)
                    return s.IsValidType(e.PropertyType);
                return true;
            }).ToArray();
        }

    }
}
