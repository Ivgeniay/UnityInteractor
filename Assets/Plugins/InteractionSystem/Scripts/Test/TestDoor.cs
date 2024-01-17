using UnityEngine;
using InteractionSystem.Test;

namespace InteractionSystem
{
    public class TestDoor : MonoBehaviour
    {
        [SerializeField] private InteractionObject interactionObject;

        //private void OnTriggerEnter(Collider other)
        //{
        //    TestMale male = other.GetComponentInChildren<TestMale>();
        //    if (male)
        //    {
        //        interactionObject.SetSubject(male.gameObject);
        //        interactionObject.StartSequence();
        //    }
        //}
    }
}
