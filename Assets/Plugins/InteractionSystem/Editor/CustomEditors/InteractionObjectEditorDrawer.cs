using UnityEditor;
using UnityEngine; 
using System;
using NodeEngine.Window;

namespace InteractionSystem
{
    [CustomEditor(typeof(InteractionObject))]
    internal class InteractionObjectEditorDrawer : Editor
    {
        InteractionObject instance;
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI ();
            instance = (InteractionObject)target;

            if (GUILayout.Button("Test"))
            {
                instance.Test();
            }
            if (GUILayout.Button("Test2"))
            {
                instance.Test2();
            }
            if (GUILayout.Button("Open Sequence Editor"))
            {
                DSEditorWindow.OpenWindow(instance);
            }
        }
    }
}
