using UnityEditor.Experimental.GraphView;
using NodeEngine.Nodes;
using NodeEngine.Ports;
using UnityEngine.PlayerLoop;
using NodeEngine.Utilities;
using NodeEngine.Window;
using UnityEngine; 

namespace InteractionSystem
{
    internal class StartNode : BaseNode
    {
        public override BaseInteractionAction InteractionAction
        {
            get
            {
                Sequence sequence = INode as Sequence;
                return graphView.Master.FirstAction;
            }
            protected set
            {
                Sequence sequence = INode as Sequence;
                graphView.Master.FirstAction = value;
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
                InteractionAction = InteractionAction,
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
                if (port.Value != otherPort.Value)
                {
                    port.Value = otherPort.Value;
                    InteractionAction = otherPort.Value;
                }
            }
        }

        public override void OnDestroyConnectionOutput(BasePort port, Edge edge)
        {
            base.OnDestroyConnectionOutput(port, edge);
            port.Value = null;
            InteractionAction = null;
        }
    }
}
