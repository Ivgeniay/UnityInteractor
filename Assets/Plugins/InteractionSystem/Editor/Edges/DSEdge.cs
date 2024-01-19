using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace NodeEngine.Edges
{
    public class DSEdge : Edge
    {
        public DSEdge() 
        {
            AddManipulators();
            AddCallbacks();
        }


        private void RemoveStyles()
        {
            this.RemoveFromClassList("DSEdge");
        }
        private void AddStyles()
        {
            this.AddToClassList("DSEdge");
        }
        private void AddCallbacks()
        { 
            //this.RegisterCallback<MouseEnterEvent>(OnEnter);
            //this.RegisterCallback<MouseLeaveEvent>(OnLeave);
        }

        private void OnLeave(MouseLeaveEvent evt)
        {
            Debug.Log("OnLeave");
            this.RemoveFromClassList("gradientAnimation");
            this.RemoveFromClassList("edge");
            string classes = string.Empty;
            GetClasses().ToList().ForEach(e => classes += e + " ");
            Debug.Log(classes);
        }

        private void OnEnter(MouseEnterEvent evt)
        {
            Debug.Log("OnEnter");
            //this.AddToClassList("DSEdge");
            this.AddToClassList("gradientAnimation");
            //ConnectionAnimation();
            string classes = string.Empty;
            GetClasses().ToList().ForEach(e => classes += e + " ");
            Debug.Log(classes);
        }

        public void AddManipulators()
        {
            this.AddManipulator(CreateEdgeContextualMenu());
        }

        internal void ConnectionAnimation()
        {
            //Debug.Log("Animation"); 
        }

        private IManipulator CreateEdgeContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(e =>
            {
                //e.menu.AppendAction("Add Token Node", a =>
                //{
                //    //TokenNode tokenNode = new(input, output);
                    
                //});
            });

            return contextualMenuManipulator;
        }
    }
}
