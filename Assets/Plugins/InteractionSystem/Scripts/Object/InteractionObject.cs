using System.Collections.Generic;
using UnityEngine;
using System;

namespace InteractionSystem
{
    public class InteractionObject : MonoBehaviour
    {
        [SerializeField]
        private Sequence Sequences;
        [SerializeField] private GameObject subject;

        private void Awake()
        {
            Sequences.SetObject(this.gameObject);
            //For test
            Sequences.SetSubject(subject);
        }

        public void AddSequence(BaseInteractionAction interactionAction)
        {
            if (Sequences == null) Sequences = new Sequence();
            Sequences.Append(interactionAction);
        }

        public void RemoveAction(BaseInteractionAction interactionAction)
        {
            if (Sequences == null) Sequences = new Sequence();
            Sequences.Remove(interactionAction);
        }

        public void RemoveAction(INode interactionAction)
        {
            if (Sequences == null) Sequences = new Sequence();
            Sequences.Remove(interactionAction);
        }

        public void SetSubject(GameObject gameObject) =>
            Sequences.SetSubject(gameObject);
        
        public void StartSequence() =>
            Sequences.StartSequence();


#if UNITY_EDITOR
        public Sequence GetSequence()
        {
            return Sequences;
        }
#endif

    }
}
