using UnityEngine.UIElements;
using NodeEngine.Utilities;
using InteractionSystem;
using NodeEngine.Window;

namespace NodeEngine.Toolbars
{
    internal class DSToolbar : BaseToolbar
    {
        private const string TOOLBAR_STYLE_LINK = "Assets/Plugins/InteractionSystem/NodeEngine/Resources/Front/NodeEngineToolbarStyles.uss";
        
        private Button startSequence;
        private Button stopSequence;
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
            this.interactionObject.GetSequence().SequenceStateEvent += SequenceStateHandler;
            this.LoadAndAddStyleSheets(TOOLBAR_STYLE_LINK);
            this.AddToClassList("ds-toolbar");

            startSequence = DSUtilities.CreateButton("StartSequence", StartSequence, new string[] { "ds-toolbar__button" });
            stopSequence = DSUtilities.CreateButton("StopSequence", StopSequence, new string[] { "ds-toolbar__button" });
            cleanButton = DSUtilities.CreateButton("Clean Graph", CleanGraph, new string[] { "ds-toolbar__button" });
            minimapButton = DSUtilities.CreateButton("Minimap", MinimapToggle, new string[] { "ds-toolbar__button" });

            this.Add(startSequence);
            this.Add(stopSequence);
            this.Add(cleanButton);
            this.Add(minimapButton);

            var started = this.interactionObject.GetSequence().SequenceState == Sequence.SequenceStateType.Started;
            startSequence.SetEnabled(!started);
            stopSequence.SetEnabled(started);
        } 

        private void SequenceStateHandler(Sequence.SequenceStateType obj)
        {
            startSequence.SetEnabled(obj != Sequence.SequenceStateType.Started);
            stopSequence.SetEnabled(obj == Sequence.SequenceStateType.Started);
        }

        private void MinimapToggle() => graphView.MiniMap.visible = !graphView.MiniMap.visible; 
        private void StartSequence() => interactionObject?.StartSequence();
        private void StopSequence() => interactionObject?.StopSequence();

        private void CleanGraph()
        {
            this.interactionObject.GetSequence().SequenceStateEvent -= SequenceStateHandler; 
            graphView.Master.Clean();
            graphView.CleanGraph();
            DSEditorWindow.OpenWindow(interactionObject);
        }

    }
}
