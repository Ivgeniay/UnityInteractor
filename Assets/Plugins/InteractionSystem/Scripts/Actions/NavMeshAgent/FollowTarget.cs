using System;
using System.Collections;
using UnityEngine.AI;

namespace InteractionSystem
{
    [Serializable]
    internal class FollowTarget : BaseNavMeshAction
    {
        private NavMeshAgent agent;
        public override void Awake()
        {
            agent = Object.GetComponent<NavMeshAgent>();
        }

        public override IEnumerator Procedure()
        {
            yield return null;
        }
    }
}
