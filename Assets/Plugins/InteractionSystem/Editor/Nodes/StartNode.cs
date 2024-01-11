using UnityEditor.Experimental.GraphView;
using NodeEngine.Nodes;
using NodeEngine.Ports;
using UnityEngine.PlayerLoop;
using NodeEngine.Window;
using UnityEngine;
using System.Linq;

namespace InteractionSystem
{
    internal class StartNode : BaseNode
    {
        public override BaseInteractionAction IAction
        {
            get
            {
                Sequence sequence = INode as Sequence;
                return sequence.FirstAction;
            }
            protected set
            {
                Sequence sequence = INode as Sequence;
                sequence.FirstAction = value;
            }
        }

        internal override void Initialize(DSGraphView graphView, Vector2 position, INode iAction)
        {
            base.Initialize(graphView, position, iAction);

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Output,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction,
                Orientation = Orientation.Horizontal,
                PortName = "NextAction"
            });
        }

        public override void OnConnectOutputPort(BasePort port, Edge edge)
        {
            base.OnConnectOutputPort(port, edge);
            BasePort otherPort = edge.input as BasePort;
            if (otherPort != null)
            {
                if (port.Value != otherPort.Value)
                {
                    port.Value = otherPort.Value;
                    IAction = otherPort.Value;
                }
            }
        }

        public override void OnDestroyConnectionOutput(BasePort port, Edge edge)
        {
            base.OnDestroyConnectionOutput(port, edge);
            port.Value = null;
            IAction = null;
        }
    }
}
