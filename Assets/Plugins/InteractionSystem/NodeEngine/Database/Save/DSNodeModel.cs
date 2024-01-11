using System.Collections.Generic;
using UnityEngine;
using System;
using NodeEngine.Nodes;

namespace NodeEngine.Database.Save
{
    [Serializable]
    public class DSNodeModel
    {
        [SerializeField] internal List<PortInfo> Outputs = new();
        [SerializeField] internal List<PortInfo> Inputs = new();
        [SerializeField] public Vector2 Position;
        [SerializeField] public string NodeName;
        [SerializeField] public string GroupID;
        [SerializeField] public string ID;

        internal void Init()
        {
            Outputs = new List<PortInfo>();
            Inputs = new List<PortInfo>();
        }

        internal void AddPort(PortInfo port)
        {
            if (port.Direction == UnityEditor.Experimental.GraphView.Direction.Input) Inputs.Add(port);
            else if (port.Direction == UnityEditor.Experimental.GraphView.Direction.Output) Outputs.Add(port);
        }

    }
}
