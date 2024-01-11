using System.Collections.Generic;
using UnityEngine;
using System;
using NodeEngine.SDictionary;

namespace NodeEngine.Database.Save
{
    [Serializable]
    public class DSGraphModel
    {
        [field: SerializeField] public string InstanceID { get; set; }
        [field: SerializeField] public List<DSGroupModel> Groups { get; set; }
        [field: SerializeField] public List<DSNodeModel> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldUGroupedNodeNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, string> anchors { get; set; } 

        public void Init(string filename)
        {
            InstanceID = filename;
            Groups = new();
            Nodes = new();
            OldGroupNames = new();
            OldUngroupedNodeNames = new();
            OldUGroupedNodeNames = new();
            anchors = new();
        }
    }
}
