using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace NodeEngine.Utilities
{
    internal static class DSStyleUtility
    {
        private static Dictionary<string, string> pathCach = new Dictionary<string, string>();

        public static VisualElement LoadAndAddStyleSheets(this VisualElement element, params string[] links)
        {
            foreach (var sheetName in links)
            {
                StyleSheet styleSheet = EditorGUIUtility.Load(sheetName) as StyleSheet;
                if (styleSheet == null) throw new Exception($"Style is not downloaded {sheetName}");
                element?.styleSheets.Add(styleSheet);
            }
            return element;
        }

        public static VisualElement LoadAndAddStyleSheetsByName(this VisualElement element, params string[] filenames)
        {
            foreach (var sheetName in filenames)
            {
                try
                {
                    if (pathCach.TryGetValue(sheetName, out string _path))
                    {
                        LoadAndAddStyleSheets(element, _path);
                        return element;
                    } 
                }
                catch (Exception e) 
                { 

                }

                string path = FileSearchScript.SearchFile(sheetName);
                if (path != null)
                {
                    pathCach[sheetName] = path;
                    LoadAndAddStyleSheets(element, path);
                }
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
