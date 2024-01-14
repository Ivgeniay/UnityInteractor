using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

namespace InteractionSystem
{
    public class Test : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            //AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            //if (controller != null)
            //{
            //    foreach (AnimatorControllerLayer layer in controller.layers)
            //    {
            //        foreach (ChildAnimatorState state in layer.stateMachine.states)
            //        {
            //            foreach (AnimatorStateTransition transition in state.state.transitions)
            //            {
            //                foreach (AnimatorCondition condition in transition.conditions)
            //                {
            //                    Debug.Log($"Condition: {state.state.name} {transition.name} {condition.parameter} {condition.mode} {condition.threshold}");
            //                }
            //            }
            //        }
            //    }
            //}


        }

        void Update()
        {
            //Debug.LogWarning(animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            //Debug.Log(animator.GetAnimatorTransitionInfo(0).duration);

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    agent.SetDestination(hitInfo.point);
                }
            }
        }
    }
}
