using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.UIElements;
using NodeEngine.Window;
using NodeEngine.Ports;
using System.Linq;
using System;

namespace NodeEngine.UIElement
{
    public class DSTextField : TextField
    {
        public Type Type { get => this.GetType();}
        public object Value { get => text; set => text = value.ToString(); }
        public bool IsFunctions { get; set; } = false;
        public string Name { get; set; } = "Text";

        private DSGraphView graphView;
        TextElement anchorsTElement;

        public void Initialize(DSGraphView graphView)
        {
            this.graphView = graphView;

            AddManipulators();

            anchorsTElement = new TextElement();
            this.Add(anchorsTElement);
            this.RegisterValueChangedCallback(OnTextFieldValueChange);
        }
        internal void OnDistroy() {
            
        }

        private void AddManipulators()
        {
            //this.AddManipulator(CreateContextualMenuAnchors());

        }
        private IManipulator CreateContextualMenuAnchors()
        {
            ContextualMenuManipulator contextualMenuManipulator = new(e =>
            {
                
            });
            return contextualMenuManipulator;
        }

        private void OnTextFieldValueChange(ChangeEvent<string> evt)
        {
            DSTextField target = evt.target as DSTextField;
            Value = evt.newValue;

            if (target != null)
            {
                
            }
        }
        
        private void OnAnchorClickEvent(TextElement textElement)
        {
            Regex regex = new Regex(@"<color=[^>]+>([^<]+)</color>");
            Match match = regex.Match(textElement.text);
            string data = "d_a_t_a";
            if (match.Success) data = match.Groups[1].Value;

            BasePort port = null;
            if (port != null)
            {
                graphView.AddToSelection(port.node);
                graphView.FrameSelection();
            }
        }
        private void OnGraphAnchorsChangedHandler(object sender, DictionaryChangedEventArgs<BasePort, string> e)
        {
            if (e.ChangeType == DictionaryChangeType.Update) OnAnchorUpdate(e.Key, e.Value);
            else if (e.ChangeType == DictionaryChangeType.Remove) OnAnchorDelete(e.Key);
            else if (e.ChangeType == DictionaryChangeType.Add)
            { }
        }
        public void OnAnchorUpdate(BasePort port, string newRegex) {}
        public void OnAnchorDelete(BasePort port) {}


    }
}
