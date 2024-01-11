using UnityEditor;
using UnityEngine; 
using System;
using NodeEngine.Window;

namespace InteractionSystem
{
    [CustomEditor(typeof(InteractionObject))]
    internal class InteractionObjectEditorDrawer : Editor
    {
        private InteractionObject instance;
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI ();
            instance = (InteractionObject)target;

            if (GUILayout.Button("Add Test InActions"))
            {
                instance.AddTestActions();
            }

            if (GUILayout.Button("Start Test Sequence"))
            {
                instance.StartSequence();
            }

            if (GUILayout.Button("Open Sequence Editor"))
            {
                DSEditorWindow.OpenWindow(instance);
            }
        }
    }
}
