using UnityEditor;
using UnityEngine.UIElements;

namespace NodeEngine.Utilities
{
    internal static class DSStyleUtility
    {
        public static VisualElement LoadAndAddStyleSheets(this VisualElement element, params string[] links)
        {
            foreach (var sheetName in links)
            {
                StyleSheet styleSheet = EditorGUIUtility.Load(sheetName) as StyleSheet;
                element?.styleSheets.Add(styleSheet);
            }
            return element;
        }

        public static VisualElement LoadAndAddStyleSheets(this VisualElement element, params StyleSheet[] styles)
        {
            foreach (StyleSheet sheet in styles)
                element?.styleSheets.Add(sheet);
            
            return element;
        }

        public static VisualElement AddToClassList(this VisualElement element, params string[] styleSheetNames)
        {
            if (styleSheetNames != null && styleSheetNames.Length > 0)
            {
                for (int i = 0; i < styleSheetNames.Length; i++)
                    element.AddToClassList(styleSheetNames[i]);
            }
            return element;
        }
    }
}
