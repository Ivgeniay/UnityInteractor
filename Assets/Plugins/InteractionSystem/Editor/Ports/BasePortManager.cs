using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeEngine.Ports
{
    public static class BasePortManager
    {
        private static List<BasePort> ports = new List<BasePort>();

        public static void UnRegister(BasePort port)
        {
            if (ports.Contains(port))
                ports.Remove(port);
        }

        public static void Register(BasePort port)
        {
            if (!ports.Contains(port))
                ports.Add(port);
        }

        public static void CallStartDrag(BasePort port)
        {
            ports.ForEach(p =>
            {
                if (p != port)
                {
                    if (HaveCommonTypes(port.Type, p.AvailableTypes)) p.SetEnabled(true);
                    else p.SetEnabled(false); 
                }
            });
        }

        public static void CallStopDrag(BasePort port)
        {
            foreach (var p in ports)
            {
                if (p != port)
                    p.SetEnabled(true);
            }
        }

        public static bool HaveCommonTypes(Type portDraggingType, Type[] otherAvaliableTypes)
        {
            if (otherAvaliableTypes.Length == 0 || portDraggingType == null) return false;

            foreach (Type type2 in otherAvaliableTypes)
            {
                if (type2 != null)
                { 
                    if (type2.IsAssignableFrom(portDraggingType)) return true;
                    if (portDraggingType.IsGenericType && type2.IsGenericType)
                    {
                        Type[] genericArgs1 = portDraggingType.GetGenericArguments();
                        Type[] genericArgs2 = type2.GetGenericArguments();

                        bool allArgumentsMatch = genericArgs1
                            .Zip(genericArgs2, (arg1, arg2) => HaveCommonTypes(arg1, new[] { arg2 }))
                            .All(result => result);
                        if (allArgumentsMatch) return true;
                    }

                    if (portDraggingType.IsArray && type2.IsArray)
                    {
                        Type elementType1 = portDraggingType.GetElementType();
                        Type elementType2 = type2.GetElementType();

                        if (IsSubset(elementType1, elementType2) || IsSubset(elementType2, elementType1)) return true;
                    } 
                }
            }
            return false;
        }

        private static bool IsSubsetOfGenericArguments(Type[] subset, Type[] superset) =>
            subset.All(subsetType => superset.Any(superType => IsSubset(subsetType, superType)));

        private static bool IsSubset(Type type1, Type type2)
        {
            if (type1 == type2) return true;
            if (type2.IsGenericType && type1.IsGenericType && type1.GetGenericTypeDefinition() == type2.GetGenericTypeDefinition())
            {
                Type[] type1GenericArgs = type1.GetGenericArguments();
                Type[] type2GenericArgs = type2.GetGenericArguments();

                return type1GenericArgs.Length == type2GenericArgs.Length &&
                       type1GenericArgs.Zip(type2GenericArgs, (arg1, arg2) => IsSubset(arg1, arg2)).All(result => result);
            }

            return false;
        }
    }
}
