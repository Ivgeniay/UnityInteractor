using UnityEditor.Experimental.GraphView;
using NodeEngine.Window;
using NodeEngine.Nodes;
using UnityEngine;
using NodeEngine.Ports;
using System.Collections.Generic;

namespace InteractionSystem
{
    internal class ActionNode : BaseNode
    {
        public override BaseInteractionAction IAction { get => INode as BaseInteractionAction; }

        protected const string ACTION_PN = "Action";
        protected const string REFERENCE_PN = "ReferenceAction";
        protected const string PARALLEL_PN = "ParallelAction";
        protected const string NEXT_PN = "NextAction";

        internal override void Initialize(DSGraphView graphView, Vector2 position, INode _iAction)
        {
            base.Initialize(graphView, position, _iAction);
           
            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction,
                Orientation = Orientation.Horizontal,
                PortName = ACTION_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction.ReferenceAction,
                Orientation = Orientation.Horizontal,
                PortName = REFERENCE_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction.ParallelAction,
                Orientation = Orientation.Horizontal,
                PortName = PARALLEL_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Output,
                Capacity = Port.Capacity.Multi,
                InteractionAction = IAction.NextIAction,
                Orientation = Orientation.Horizontal,
                PortName = NEXT_PN
            });
        }

        public override void OnConnectOutputPort(BasePort port, Edge edge)
        {
            base.OnConnectOutputPort(port, edge);
            BasePort otherPort = edge.input as BasePort;
            if (otherPort != null)
            {
                if (otherPort.Name == ACTION_PN)
                {
                    //if (port.Value != otherPort.Value)
                    //{
                        port.Value = otherPort.Value;
                        IAction.NextIAction = port.Value;
                    //}
                }
            }
        }

        public override void OnDestroyConnectionOutput(BasePort port, Edge edge)
        {
            base.OnDestroyConnectionOutput(port, edge);

            port.Value = null;
            IAction.NextIAction = null;
        }

        public override void OnConnectInputPort(BasePort port, Edge edge)
        {
            base.OnConnectInputPort(port, edge);

            BaseNode otherNode = edge.output.node as BaseNode;
            if (otherNode != null)
            {
                if (port.Name == REFERENCE_PN)
                {
                    port.Value = otherNode.IAction;
                    IAction.ReferenceAction = port.Value;
                }
                else if (port.Name == PARALLEL_PN)
                {
                    port.Value = otherNode.IAction;
                    IAction.ParallelAction = port.Value;
                }
            }
        }

        public override void OnDestroyConnectionInput(BasePort port, Edge edge)
        {
            base.OnDestroyConnectionInput(port, edge);

            if (port.Name == REFERENCE_PN)
                IAction.ReferenceAction = null;

            else if (port.Name == PARALLEL_PN)
                IAction.ParallelAction = null;
        }
    }
}
