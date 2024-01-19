using UnityEngine;

namespace InteractionSystem
{
    /// <summary>
    /// The interface representing a contract on the basis of which the data used to display nodes in GrathView is stored 
    /// </summary>
    public interface INode
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public string ID { get; set; }
    }
}
