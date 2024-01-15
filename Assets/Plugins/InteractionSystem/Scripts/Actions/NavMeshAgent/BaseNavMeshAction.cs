using System;
using UnityEngine.AI;

namespace InteractionSystem
{
    [Serializable]
    public abstract class BaseNavMeshAction : BaseInteractionAction
    {
        protected NavMeshAgent agent;
        public override void Awake()
        {
            agent = Performer.GetComponentInChildren<NavMeshAgent>();
        }
    }
}
