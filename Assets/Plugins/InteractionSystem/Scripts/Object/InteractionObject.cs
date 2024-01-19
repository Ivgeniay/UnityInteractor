using System.Linq;
using UnityEngine;

namespace InteractionSystem
{
    public class InteractionObject : MonoBehaviour
    {
        [SerializeField] private Sequence Sequences;
        [SerializeField] private GameObject subject;

        private void Awake() => Sequences.SetObject(this.gameObject);
        public void SetSubject(GameObject gameObject) => Sequences.SetSubject(gameObject);
        public void StartSequence()
        {
            if (Sequences.Subject == null) Sequences.SetSubject(subject);
            Sequences.StartSequence();
        }
        public void StopSequence()
        {
            if (Sequences.Subject == null) Sequences.SetSubject(subject);
            Sequences.StopSequence();
        }


#if UNITY_EDITOR
        public Sequence GetSequence() => Sequences;
#endif

    }
}
