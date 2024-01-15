using System.Collections;
using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода управляет NavMesh агентом. Отправляет его в заданную точку и дожидается когда дистанция с точкой станет меньше или равна minimalDistance")]
    public class FollowToPoint : BaseNavMeshAction
    {
        [SerializeField] 
        [SerializeFieldNode]
        private float minimalDistance = 0.3f;

        [SerializeField] 
        [SerializeFieldNode]
        private Vector3 followPosition; 

        protected override IEnumerator Procedure()
        {
            if (ReferenceAction != null)
            {
                switch (ReferenceAction)
                {
                    case RememberDestinationPoint rememberDestinationPoint:
                        followPosition = rememberDestinationPoint.ValuePosition;
                        break;

                    case VectorThree vectorThree: 
                        followPosition = vectorThree.ValuePosition; 
                        break;

                    case UnityObject unityObject:
                        followPosition = unityObject.Value.transform.position;
                        break;
                }
            }
            agent.SetDestination(followPosition);
            yield return new WaitUntil(() => !agent.pathPending);
            yield return new WaitUntil(() =>
            {
                Debug.Log($"rem: {agent.remainingDistance} min: {minimalDistance}");
                return agent.remainingDistance <= minimalDistance;
            });
        }
    }
}
