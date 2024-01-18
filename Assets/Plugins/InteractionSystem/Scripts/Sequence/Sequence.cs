using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;

namespace InteractionSystem
{
    [Serializable]
    public class Sequence : INode
    {
        public event Action<SequenceStateType> SequenceStateEvent;
        public event Action<BaseInteractionAction, ActionExecutionType> ActionExecutionEvent;
        public SequenceStateType SequenceState = SequenceStateType.Ended;
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
                Sequences.Add(sequence);

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
        public IEnumerable<T> Get<T>() where T : BaseInteractionAction
        {
            foreach (var action in Sequences)
                if (action is T)
                    yield return (T)action;
        }
        public Sequence StartSequence()
        {
            if (IsProgress)
            {
                Debug.LogError($"Sequence is in progress");
                return this;
            }

            InvokeSequenceStatus(SequenceStateType.Started);
            if (currentSequence != null) coroutine.StopC(currentSequence);
            currentSequence = coroutine.StartC(StartActions());

            return this;
        }
        public Sequence StopSequence()
        {
            if (currentSequence != null) coroutine.StopC(currentSequence);
            Sequences.ForEach(sequence => { 
                sequence.OnStop();
                sequence.Reset();
                sequence.OnExecutingEvent -= OnExecutingActionHandler; 
            });
            InvokeSequenceStatus(SequenceStateType.Stopped);

            return this;
        }
        internal void Clean()
        {
            if (IsProgress != false)
            {
                Debug.LogError("Stoping of sequnce is inpossible when sequence in progress. Call StopSequence.");
                return;
            }
            Sequences.ForEach (sequence => { sequence.OnExecutingEvent -= OnExecutingActionHandler; });
            Sequences.Clear();
            FirstAction = null;
        }
        private void InvokeSequenceStatus(SequenceStateType type)
        {
            IsProgress = type == SequenceStateType.Started;
            SequenceState = type;
            SequenceStateEvent?.Invoke(type);
        }
        private IEnumerator StartActions()
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
                    sequence.OnExecutingEvent += OnExecutingActionHandler;
                });

                Sequences.ForEach(sequence => sequence.Awake());
            } 
            yield return FirstAction.MainProcedure();

            foreach (var item in Sequences) item.Reset();
            InvokeSequenceStatus(SequenceStateType.Ended);
        } 
        private void OnExecutingActionHandler(BaseInteractionAction arg1, ActionExecutionType arg2) =>
            ActionExecutionEvent?.Invoke(arg1, arg2);
        

        public enum SequenceStateType { Started, Ended, Stopped }
        public enum ActionExecutionType { Waiting, Awake, Procedure, Exit, WaitParallel, OnStop, Complete }
    }

    public interface INode
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public string ID { get; set; }
    }
}
