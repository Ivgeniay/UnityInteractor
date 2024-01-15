using UnityEngine.AI;
using UnityEngine;

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
