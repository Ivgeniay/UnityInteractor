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
                
                var res = GetPrimitiveTypeAttributed(type);
                if (res.fields.Length > 0) FieldsAttributed[type] = res.fields;
                if (res.property.Length > 0) PropertyAttributed[type] = res.property;
            }

            var enums = GetEnumAttributedFields();
            if (enums.fields.Length > 0) FieldsAttributed[typeof(EnumFieldContextAttribute)] = enums.fields;
            if (enums.property.Length > 0) PropertyAttributed[typeof(EnumFieldContextAttribute)] = enums.property;
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

        private (FieldInfo[] fields, PropertyInfo[] property) GetPrimitiveTypeAttributed(Type attributes)
        {
            Type type = instance.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, attributes))
                .ToArray();

            PropertyInfo[] property = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, attributes))
                .ToArray();

            return (
                fields.Where(e =>
                    {
                        DSAttribute dsAttribute = (DSAttribute)Attribute.GetCustomAttribute(e, typeof(DSAttribute));
                        if (dsAttribute is SerializedDSAttribute s)
                        {
                            bool result = s.IsValidType(e.FieldType);
                            return result;
                        }
                        return true;
                    }).ToArray()
                    ,
                property.Where(e =>
                    {
                        DSAttribute dsAttribute = (DSAttribute)Attribute.GetCustomAttribute(e, typeof(DSAttribute));
                        if (dsAttribute is SerializedDSAttribute s)
                            return s.IsValidType(e.PropertyType);
                        return true;
                    }).ToArray()
            );
        }
        private (FieldInfo[] fields, PropertyInfo[] property) GetEnumAttributedFields()
        {
            Type type = instance.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(EnumFieldContextAttribute)))
                .ToArray();

            PropertyInfo[] property = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined (prop, typeof(EnumFieldContextAttribute))) 
                .ToArray();

            return (fields, property);
        }
    }
}
