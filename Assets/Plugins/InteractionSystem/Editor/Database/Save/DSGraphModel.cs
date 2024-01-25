using System.Collections.Generic;
using NodeEngine.SDictionary;
using UnityEngine;
using System;

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
            Groups = new List<DSGroupModel>();
            Nodes = new List<DSNodeModel>();
            OldGroupNames = new List<string>();
            OldUngroupedNodeNames = new List<string>();
            OldUGroupedNodeNames = new SerializableDictionary<string, List<string>>();
            anchors = new SerializableDictionary<string, string>();
        }
    }
}
