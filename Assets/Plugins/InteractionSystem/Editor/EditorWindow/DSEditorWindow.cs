using InteractionSystem;
using NodeEngine.Toolbars;
using NodeEngine.Utilities;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEngine.Window
{
    public class DSEditorWindow : EditorWindow
    {
        public StyleSheet StyleSheet;
        private string stylesLink = "Assets/Plugins/InteractionSystem/NodeEngine/Resources/Front/NodeEngineVariables.uss";

        private DSGraphView grathView;
        public InteractionObject InteractionInstance { get; private set; }

        public static void OpenWindow(InteractionObject interactionObject)
        {
            DSEditorWindow dSEditor = GetWindow<DSEditorWindow>();
            dSEditor.Initialize(interactionObject);
        } 

        internal void Initialize(InteractionObject interactionObject)
        {
            titleContent = new GUIContent(interactionObject.gameObject.name);
            InteractionInstance = interactionObject;
            AddGraphView();
            AddToolbar();
            AddStyles();

            grathView.Initialize();
        }
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectNewObject;
        }
        private void OnDisable()
        {
            grathView.OnDestroy();
            Selection.selectionChanged -= OnSelectNewObject;
        }
        private void OnPlayModeStateChanged(PlayModeStateChange state) {}
        private void OnSelectNewObject()
        {
            foreach (var selectedObject in Selection.objects)
            {
                GameObject selectedGameObject = selectedObject as GameObject;
                if (selectedGameObject != null)
                {
                    InteractionObject interaction = selectedGameObject.GetComponent<InteractionObject>();
                    if (interaction != null)
                    {
                        OpenWindow(interaction);
                    }
                }
            }
        }


        #region Elements Addition
        private void AddGraphView()
        {
            grathView = new DSGraphView(this);
            grathView.StretchToParentSize();

            rootVisualElement.Add(grathView);
        }
        private void AddToolbar()
        {
            DSToolbar toolbar = new(grathView);
            toolbar.Initialize(InteractionInstance);
            rootVisualElement.Add(toolbar);
        }
        #endregion

        #region Styles
        private void AddStyles()
        {
            if (StyleSheet == null) rootVisualElement.LoadAndAddStyleSheets(stylesLink);
            else rootVisualElement.LoadAndAddStyleSheets(StyleSheet);
        }
        #endregion

    }
}
