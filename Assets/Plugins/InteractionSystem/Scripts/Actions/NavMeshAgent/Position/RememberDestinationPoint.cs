using System;
using System.Collections;
using UnityEngine;

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода запоминает позицию куда направляется NavMesh в момент её срабатывания и хранит её.")]
    public class RememberDestinationPoint : BaseNavMeshAction
    {
        [SerializeFieldNode] public Vector3 ValuePosition;

        protected override IEnumerator Procedure()
        {
            ValuePosition = agent.destination;
            if (false) yield return null;
        }
    }
}
