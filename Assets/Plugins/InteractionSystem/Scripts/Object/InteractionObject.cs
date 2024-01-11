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
        }


        public void Test()
        {
            if (Sequences == null) Sequences = new();

            BaseInteractionAction animatorAction = new AnimatorAction()
                .OnComplete(() => Debug.Log($"Yoyoyoy from 1"));
            BaseInteractionAction animatorAction2 = new AnimatorAction()
                .OnComplete(() => Debug.Log($"Yoyoyoy from 2"));
            animatorAction.NextAction(animatorAction2);

            Sequences
                .Append(animatorAction)
                .Append(animatorAction2)
                .SetSubject(subject)
                .StartSequence();
        }
        public void Test2()
        {
            BaseInteractionAction animatorAction = new AnimatorAction()
                .OnComplete(() => Debug.Log($"Yoyoyoy from new AnimatorAction"));
            Sequences
                .Append(animatorAction);
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
