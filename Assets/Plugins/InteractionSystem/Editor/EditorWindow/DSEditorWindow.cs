using UnityEngine.UIElements;
using NodeEngine.Utilities; 
using NodeEngine.Toolbars;
using InteractionSystem;
using UnityEditor;
using UnityEngine;

namespace NodeEngine.Window
{
    public class DSEditorWindow : EditorWindow
    {
        public StyleSheet StyleSheet;
        public InteractionObject InteractionInstance { get; private set; }
        private DSGraphView grathView;

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
            DSToolbar toolbar = new DSToolbar(grathView);
            toolbar.Initialize(InteractionInstance);
            rootVisualElement.Add(toolbar);
        }
        #endregion

        #region Styles
        private void AddStyles()
        {
            if (StyleSheet == null) rootVisualElement.LoadAndAddStyleSheetsByName(DSConstants.VARIABLE_LINK);
            else rootVisualElement.LoadAndAddStyleSheets(StyleSheet);
        }
        #endregion

    }
}
