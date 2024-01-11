using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEngine.Database.Save
{
    [Serializable]
    public class NodePortModel
    {
        [SerializeField] public string NodeID;
        [SerializeField] public List<string> PortIDs;
    }
}
