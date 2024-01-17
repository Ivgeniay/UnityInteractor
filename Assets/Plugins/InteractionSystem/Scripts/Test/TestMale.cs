using UnityEngine.AI;
using UnityEngine;

namespace InteractionSystem.Test
{
    internal class TestMale : MonoBehaviour
    {
        private int Speed = Animator.StringToHash("Speed");

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float MaxSpeed = 2;
        [SerializeField] private Animator animator;

        private void Awake()
        {
            agent.speed = MaxSpeed;
        }

        private void Update()
        {
            animator.SetFloat(Speed, Mathf.InverseLerp(0, MaxSpeed, agent.velocity.magnitude));
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
