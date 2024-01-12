using UnityEditor.Experimental.GraphView;
using NodeEngine.Window;
using NodeEngine.Nodes;
using UnityEngine;
using NodeEngine.Ports;
using System.Collections.Generic;
using NodeEngine.Utilities;

namespace InteractionSystem
{
    internal class ActionNode : BaseNode
    {
        public override BaseInteractionAction IAction { get => INode as BaseInteractionAction; }

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
                PortName = DSConstants.ACTION_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction.ReferenceAction,
                Orientation = Orientation.Horizontal,
                PortName = DSConstants.REFERENCE_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction.ParallelAction,
                Orientation = Orientation.Horizontal,
                PortName = DSConstants.PARALLEL_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Output,
                Capacity = Port.Capacity.Multi,
                InteractionAction = IAction.NextIAction,
                Orientation = Orientation.Horizontal,
                PortName = DSConstants.NEXT_PN
            });
        }

        public override void OnConnectOutputPort(BasePort port, Edge edge)
        {
            base.OnConnectOutputPort(port, edge);
            BasePort otherPort = edge.input as BasePort;
            if (otherPort != null)
            {
                if (otherPort.Name == DSConstants.ACTION_PN)
                {
                    port.Value = otherPort.Value;
                    IAction.NextIAction = port.Value;
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
                if (port.Name == DSConstants.REFERENCE_PN)
                {
                    port.Value = otherNode.IAction;
                    IAction.ReferenceAction = port.Value;
                }
                else if (port.Name == DSConstants.PARALLEL_PN)
                {
                    port.Value = otherNode.IAction;
                    IAction.ParallelAction = port.Value;
                }
            }
        }

        public override void OnDestroyConnectionInput(BasePort port, Edge edge)
        {
            base.OnDestroyConnectionInput(port, edge);

            if (port.Name == DSConstants.REFERENCE_PN)
                IAction.ReferenceAction = null;

            else if (port.Name == DSConstants.PARALLEL_PN)
                IAction.ParallelAction = null;
        }
    }
}
