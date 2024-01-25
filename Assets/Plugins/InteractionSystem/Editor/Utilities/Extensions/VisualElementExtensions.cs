using System;
using System.Collections.Generic; 
using UnityEngine.UIElements;

namespace NodeEngine
{
    internal static class VisualElementExtensions
    { 
        public static List<T> GetElementsByType<T>(this VisualElement root, Func<object, bool> predicate = null)
        {
            List<T> elementsOfType = new List<T>();
            CollectElementsByType(root, elementsOfType, predicate);

            return elementsOfType;
        }

        private static void CollectElementsByType<T>(VisualElement element, List<T> elementsOfType, Func<object, bool> predicate = null)
        {
            if (predicate == null)
            {
                if (element is T typedElement)
                    elementsOfType.Add(typedElement);
            }
            else
            {
                if (element is T typedElement && predicate(typedElement))
                    elementsOfType.Add(typedElement);
            }

            foreach (var child in element.Children())
            {
                CollectElementsByType(child, elementsOfType, predicate);
            }
        }
    }
}
