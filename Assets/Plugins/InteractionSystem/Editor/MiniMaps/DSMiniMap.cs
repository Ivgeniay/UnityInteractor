using NodeEngine.Utilities;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace NodeEngine.MiniMaps
{
    internal class DSMiniMap : MiniMap
    {
        //private const string MINIMAP_STYLE_LINK = "Assets/Plugins/DialogueSystem/Resources/Front/DialogueSystemMinimapStyles.uss";
    
        public DSMiniMap()
        {
            AddStyles();
        }

        private void AddStyles()
        {
            StyleColor backgroundColor = new StyleColor(new UnityEngine.Color32(10, 10, 10, 60));
            StyleColor borderColor = new StyleColor(new UnityEngine.Color32(10, 10, 10, 190));

            style.backgroundColor = backgroundColor;
            style.borderLeftColor = borderColor;
            style.borderRightColor = borderColor;
            style.borderTopColor = borderColor;
            style.borderBottomColor = borderColor;
        }
    }
}
