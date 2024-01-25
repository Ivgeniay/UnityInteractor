using UnityEditor.Experimental.GraphView;
using InteractionSystem;
using System;

namespace NodeEngine.Nodes
{
    public class PortInfo
    {
        internal string PortName;
        internal Type Type;
        internal Port.Capacity Capacity;
        internal Direction Direction;
        internal Orientation Orientation;
        internal BaseInteractionAction InteractionAction;
        internal BaseNode Node;
    }
}
