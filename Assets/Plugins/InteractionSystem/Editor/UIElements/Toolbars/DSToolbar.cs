using InteractionSystem;
using NodeEngine.Text;
using NodeEngine.Utilities;
using NodeEngine.Window;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEngine.Toolbars
{
    internal class DSToolbar : BaseToolbar
    {
        private const string TOOLBAR_STYLE_LINK = "Assets/Plugins/InteractionSystem/NodeEngine/Resources/Front/NodeEngineToolbarStyles.uss";
        
        private Button startSequence;
        private Button cleanButton;
        private Button minimapButton;

        private DSGraphView graphView;
        private InteractionObject interactionObject;
        public DSToolbar(DSGraphView graphView) 
        {
            this.graphView = graphView; 
        }

        public void Initialize(InteractionObject interactionObject)
        {
            this.interactionObject = interactionObject;
            this.LoadAndAddStyleSheets(TOOLBAR_STYLE_LINK);
            this.AddToClassList("ds-toolbar");

            
            startSequence = DSUtilities.CreateButton("StartSequence", StartSequence, new string[]
            {
                "ds-toolbar__button"
            });
            cleanButton = DSUtilities.CreateButton("Clean Graph", CleanGraph, new string[]
            {
                "ds-toolbar__button"
            });
            minimapButton = DSUtilities.CreateButton("Minimap", MinimapToggle, new string[]
            {
                "ds-toolbar__button"
            });

            this.Add(startSequence);
            this.Add(cleanButton);
            this.Add(minimapButton);
        }

        private void MinimapToggle() => graphView.MiniMap.visible = !graphView.MiniMap.visible;

        private void StartSequence()
        {
            interactionObject?.StartSequence();
        }

        private void CleanGraph()
        {
            interactionObject.CleanSequence();

            graphView.CleanGraph();
            DSEditorWindow.OpenWindow(interactionObject);
        }

    }
}
