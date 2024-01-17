using System.Collections;
using UnityEngine;

namespace InteractionSystem
{
    internal class RotateAgent : BaseNavMeshAction
    {
        [SerializeFieldNode]
        [SerializeField]
        private Vector3 EulerEngle = new Vector3(0, 90, 0);

        [SerializeFieldNode]
        [SerializeField]
        private float RotationTime = 0.5f;

        protected override IEnumerator Procedure()
        {
            Quaternion targetRotation = Quaternion.identity;

            if (ReferenceAction != null)
            {
                switch (ReferenceAction)
                {
                    case VectorThree vectorThree:
                        EulerEngle = vectorThree.ValuePosition;
                        targetRotation = Quaternion.Euler(EulerEngle);
                        break;

                    case UnityObject unityObject:
                        Vector3 targetDirection = unityObject.Value.transform.position - agent.transform.position;
                        targetDirection.y = 0f;
                        targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                        break;
                }
            }

            float elapsedTime = 0f;
            Quaternion startRotation = agent.transform.rotation;

            while (elapsedTime < RotationTime)
            {
                float t = elapsedTime / RotationTime;
                agent.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            agent.transform.rotation = targetRotation;
        }
    }
}
