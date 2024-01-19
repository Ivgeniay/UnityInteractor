using UnityEditor.Experimental.GraphView;
using NodeEngine.Manipulations;
using UnityEngine.UIElements;
using InteractionSystem;
using System.Reflection;
using NodeEngine.Window;
using NodeEngine.Edges;
using UnityEditor;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace NodeEngine.Ports
{
    public class BasePort : Port
    {
        public string ID { get
            {
                if (Value != null) return Value.ID;
                else return id;
            }
            set => id = value; 
        }
        public BaseInteractionAction Value { get; set; }
        public Type Type { get => portType; set => portType = value; }
        public string Name { get => portName; set => portName = value; }
        public DSGraphView GrathView { get; internal set; }
        public Type[] AvailableTypes;
        private string id = string.Empty;
        private Color defaultColor;
        
        public BasePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type) { }
        public static BasePort CreateBasePort<TEdge>(Orientation orientation, Direction direction, Capacity capacity, Type type) where TEdge : DSEdge, new()
        {
            Type edgeConnectorType = typeof(CustomEdgeConnector<>).MakeGenericType(typeof(TEdge));
            Type outerType = typeof(Port);
            Type innerType = outerType.GetNestedType("DefaultEdgeConnectorListener", BindingFlags.NonPublic);

            object listenerInstance = Activator.CreateInstance(innerType);
            object edgeConnectorInstance = Activator.CreateInstance(edgeConnectorType, listenerInstance);

            BasePort port = new BasePort(orientation, direction, capacity, type);

            Type Type = typeof(BasePort);
            FieldInfo edgeConnectorField = Type.GetField("m_EdgeConnector", BindingFlags.NonPublic | BindingFlags.Instance);

            if (edgeConnectorField != null)
                edgeConnectorField.SetValue(port, edgeConnectorInstance);

            port.AddManipulators();
            port.defaultColor = port.portColor;
            EditorApplication.update += port.Update;
            return port;
        }

        private void Update() { }
        internal void OnDistroy()
        {
            EditorApplication.update -= Update;
            BasePortManager.UnRegister(this);
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);
            if (edge.output != null && edge.input == this)
            {
                BasePort other = edge.output as BasePort;
                if (Type == other.Type) return;
                if (!BasePortManager.HaveCommonTypes(other.Type, AvailableTypes))
                {
                    Disconnect(edge);
                    edge.output.portCapLit = false;
                    edge.input.portCapLit = false;
                }
            }
        }
        public override void Disconnect(Edge edge)
        {
            if (edge != null)
            {
                GraphView graphView = GetFirstAncestorOfType<GraphView>();
                if (graphView != null)
                {
                    var t = new GraphViewChange();
                    t.elementsToRemove = new List<GraphElement>() { edge };
                    graphView.graphViewChanged?.Invoke(t);
                    graphView.RemoveElement(edge);
                }
            }
            base.Disconnect(edge);
        }

        public override void DisconnectAll()
        {
            var connect = connections.ToList();
            for (int i = 0; i < connect.Count(); i++)
            {
                BasePort inp = connect[i].input as BasePort;
                BasePort outp = connect[i].output as BasePort;
                inp.Disconnect(connect[i]);
                outp.Disconnect(connect[i]);
            }

            base.DisconnectAll();
        }

        public override void OnStartEdgeDragging()
        {
            if (this.m_EdgeConnector?.edgeDragHelper?.draggedPort == this)
                BasePortManager.CallStartDrag(this);
            base.OnStartEdgeDragging();
        }
        public override void OnStopEdgeDragging()
        {
            base.OnStopEdgeDragging();
            
            if (this.m_EdgeConnector?.edgeDragHelper?.draggedPort == this)
                BasePortManager.CallStopDrag(this);
        }

        private void AddManipulators()
        {
            this.AddManipulator(this.m_EdgeConnector);
            StartDragManipulator startDrag = new StartDragManipulator(this);
            this.AddManipulator(startDrag);

            //this.AddManipulator(CreateContextualMenu());
        }

        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(e =>
            { 
                e.menu.AppendAction("Anchor", a => { DSAnchorWindow.OpenWindow(this); }); 
            });
            return contextualMenuManipulator;
        }

    }
}
