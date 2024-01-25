using UnityEngine.UIElements;
using NodeEngine.Utilities; 
using NodeEngine.Toolbars;
using InteractionSystem;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using NodeEngine.Groups;

namespace NodeEngine.Window
{
    public class DSEditorWindow : EditorWindow
    {
        public StyleSheet StyleSheet;
        public InteractionObject InteractionInstance { get; private set; }

        private DSGraphView grathView;
        private DSToolbar toolbar;
        private Blackboard blackboard;

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
            AddBlackboard();
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
            grathView?.OnDestroy();
            rootVisualElement.Remove(toolbar);
            rootVisualElement.Remove(blackboard);
            Selection.selectionChanged -= OnSelectNewObject;
        }
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
            toolbar = new DSToolbar(grathView);
            toolbar.Initialize(InteractionInstance);
            rootVisualElement.Add(toolbar);
        }

        private void AddBlackboard(bool enable = false)
        {
            blackboard = new Blackboard(grathView);
            blackboard.title = "Interaction Objects";
            blackboard.subTitle = "On Active scene"; 
            blackboard.scrollable = true;

            var io = FindObjectsOfType<InteractionObject>();

            foreach (InteractionObject interaction in io)
            {
                var e = new ObjectField();
                e.value = interaction;
                e.SetEnabled(false);
                blackboard.Add(e);
            }

            //blackboard.addItemRequested += (e) => { Debug.Log("Plus"); };
            //blackboard.moveItemRequested += (e, t, r) => { Debug.Log($"Move item event: {e} {t} {r}"); };
            //blackboard.editTextRequested += (e, t, r) => { Debug.Log($"Edit text: {e} {t} {r}"); };

            blackboard.AddManipulator(CreateBlackboardContextualMenu());

            rootVisualElement.Add(blackboard);

            EnableBlackboard(enable);
        }
        internal void EnableBlackboard(bool enable) =>
            blackboard.style.display = enable == false ? DisplayStyle.None : DisplayStyle.Flex;
        internal bool IsEnabledBlackboard()
        {
            if (blackboard.style.display == DisplayStyle.None) return false;
            return true;
        }

        #endregion

        #region Manipulators
        private IManipulator CreateBlackboardContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(e =>
            {
                e.menu.AppendAction("Disable", a =>
                {
                    EnableBlackboard(!IsEnabledBlackboard());
                });
            });

            return contextualMenuManipulator;
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
