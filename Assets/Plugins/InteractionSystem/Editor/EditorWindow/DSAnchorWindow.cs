using NodeEngine.Ports;
using NodeEngine.Utilities;
using UnityEngine.UIElements;
using UnityEditor;
using NodeEngine.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace NodeEngine.Window
{
    internal class DSAnchorWindow : EditorWindow
    {
        public BasePort Port;

        public StyleSheet GraphStyleSheet;
        public StyleSheet NodeStyleSheet;
        public StyleSheet StylesSheet;

        private Button accptBtn;
        private Button cnlBtn;
        private TextField textField;

        public static void OpenWindow(BasePort port)
        {
            DSAnchorWindow instance = GetWindow<DSAnchorWindow>("Anchor editor");
            instance.maxSize = new UnityEngine.Vector2(400, 70);
            instance.minSize = new UnityEngine.Vector2(100, 70);
            instance.Port = port;

            instance.OnEnabled();
        }

        internal void OnEnabled()
        {
            AddElements();
            AddStyles();
        }

        #region Elements Addition
        private void AddElements()
        {
            VisualElement btnGroup = new VisualElement();
            BaseNode node = Port.node as BaseNode;
            //textField = DSUtilities.CreateTextField(
            //    $"{Port.Anchor}", 
            //    "Anchor Name:",
            //    onChange: (value) =>
            //    {
            //        TextField target = value.target as TextField;
            //        target.value = value.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            //        if (string.IsNullOrEmpty(target.value) && string.IsNullOrWhiteSpace(Port.Anchor)) accptBtn.text = "Save Null Anchor";
            //        else if (string.IsNullOrEmpty(target.value) && !string.IsNullOrWhiteSpace(Port.Anchor)) accptBtn.text = $"Delete Anchor ({Port.Anchor})";
            //        else if (!string.IsNullOrEmpty(target.value) && !string.IsNullOrWhiteSpace(Port.Anchor)) accptBtn.text = "Update Anchor";
            //        else if (!string.IsNullOrEmpty(target.value) && string.IsNullOrWhiteSpace(Port.Anchor)) accptBtn.text = "Add Anchor";
            //    },
            //    styles: new string[]
            //        {
            //            "ds-node__textfield",
            //            "ds-node__filename-textfield",
            //            "ds-node__textfield__hidden"
            //        });

            //accptBtn = DSUtilities.CreateButton(
            //    "Add Anchor", 
            //    onClick: () =>
            //    {
            //        Port.AddOrUpdateAnchor(textField.value);
            //        this.Close();
            //    },
            //    styles: new string[]
            //    {
            //        "ds-node__button"
            //    });
            //cnlBtn = DSUtilities.CreateButton(
            //    "Cancel",
            //    onClick: () =>
            //    {
            //        this.Close();
            //    },
            //    styles: new string[]
            //    {
            //        "ds-node__button"
            //    });

            //btnGroup.Add(accptBtn);
            //btnGroup.Add(cnlBtn);

            //btnGroup.style.flexGrow = 1;
            //btnGroup.style.minWidth = 100;
            //btnGroup.style.height = 20;
            //btnGroup.style.flexDirection = FlexDirection.Row;

            rootVisualElement.Add(textField);
            rootVisualElement.Add(btnGroup);
        }

        private void OnDestroy()
        {
            List<VisualElement> childs = rootVisualElement.Children().ToList();
            for (int i = 0; i < childs.Count(); i++)
                rootVisualElement.Remove(childs[i]);
        }

        #endregion

        #region Styles
        private void AddStyles()
        {
            if (GraphStyleSheet == null) rootVisualElement.LoadAndAddStyleSheetsByName(DSConstants.GRAPH_STYLE_LINK);
            else rootVisualElement.LoadAndAddStyleSheets(GraphStyleSheet);

            if (NodeStyleSheet == null) rootVisualElement.LoadAndAddStyleSheetsByName(DSConstants.NODE_STYLE_LINK);
            else rootVisualElement.LoadAndAddStyleSheets(NodeStyleSheet);

            if (StylesSheet == null) rootVisualElement.LoadAndAddStyleSheetsByName(DSConstants.VARIABLE_LINK);
            else rootVisualElement.LoadAndAddStyleSheets(StylesSheet);

            rootVisualElement.AddToClassList("ds-node__main-container");
        }
        #endregion

    }
}
