using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    public class Sequence : INode
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

        [SerializeReference] public BaseInteractionAction FirstAction;
        [SerializeReference] public List<BaseInteractionAction> Sequences;
        [NonSerialized][HideInInspector] public GameObject Object;
        [NonSerialized][HideInInspector] public GameObject Subject;

        private Coroutine currentSequence = null;
        private CoroutineDisposer coroutine { get => CoroutineDisposer.Instance; } 
        [field: SerializeField] public bool IsProgress { get; private set; }

        public Sequence()
        {
            if (string.IsNullOrEmpty(ID)) ID = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(Name)) Name = GetType().Name;
            if (Sequences == null) Sequences = new();
            Position = new Vector2(350, 200);
        }

        public Sequence Append(BaseInteractionAction sequence)
        {
            if (!Sequences.Contains(sequence))
            {
                Sequences.Add(sequence); 
            }

            return this;
        }

        public Sequence Remove(BaseInteractionAction sequence)
        {
            if (Sequences.Contains(sequence))
                Sequences.Remove(sequence);

            return this;
        }

        public Sequence Remove(INode sequence)
        {
            if (sequence is BaseInteractionAction e)
            {
                if (Sequences.Contains(e))
                    Sequences.Remove(e); 
            }

            return this;
        }
        public Sequence SetSubject(GameObject subject)
        {
            Subject = subject;
            return this;
        }
        public Sequence SetObject(GameObject _object)
        {
            Object = _object;
            return this;
        }

        public List<BaseInteractionAction> Build()
        {
            return Sequences;
        }

        internal Sequence StartSequence()
        {
            if (IsProgress)
            {
                Debug.LogError($"Sequence is in progress");
                return this;
            }

            IsProgress = true;
            if (currentSequence != null) coroutine.StopC(currentSequence);
            currentSequence = coroutine.StartC(StartActions());

            return this;
        }

        public IEnumerator StartActions()
        {
            if (Sequences.Count > 0)
            {
                Sequences.ForEach(sequence =>
                {
                    switch (sequence.PerformerType)
                    {
                        case PerformerType.Object:
                            sequence.Performer = Object;
                            break;
                        case PerformerType.Subject:
                            sequence.Performer = Subject;
                            break;
                    }
                    sequence.Awake();
                });
            }

            yield return FirstAction.MainProcedure();

            foreach (var item in Sequences)
                item.Reset();

            IsProgress =false;
        }

        internal void Clean()
        {
            Sequences.Clear();
            FirstAction = null;
        }
    }

    public interface INode
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public string ID { get; set; }
    }
}
