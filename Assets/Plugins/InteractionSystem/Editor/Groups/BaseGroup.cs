using NodeEngine.Database.Save;
using NodeEngine.Nodes;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEngine.Groups
{
    public class BaseGroup : Group
    {
        private Color defaultBorderColor;
        private float defaultBorderWidth;
        private List<BaseNode> nodes = new List<BaseNode>();
        public DSGroupModel Model { get; protected set; }

        public BaseGroup(string groupTitle, Vector2 position)
        {
            title = groupTitle;

            Model = new DSGroupModel()
            {
                ID = Guid.NewGuid().ToString(),
                Type = GetType().ToString(),
                GroupName = groupTitle,
                Position = this.GetPosition().position
            };

            SetPosition(new Rect(position, Vector2.zero));

            defaultBorderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        #region Styles
        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }

        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = defaultBorderColor;
            contentContainer.style.borderBottomWidth = defaultBorderWidth;
        }
        #endregion

        #region Mono
        internal virtual void OnTitleChanged(string title) 
        {
            Model.GroupName = title;
        }
        internal virtual void OnCreate(List<BaseNode> innerNode){}
        internal virtual void OnDestroy(){}
        internal void OnChangePosition(Vector2 position, Vector2 delta) 
        {
            Model.Position = position;
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].OnChangePosition(position, delta);
        }

        internal void OnNodeAdded(BaseNode node) => nodes.Add(node);
        internal void OnNodeRemove(BaseNode node) => nodes.Remove(node);
        
        #endregion
    }
}
