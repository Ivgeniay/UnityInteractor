using System.Collections.Generic;
using UnityEngine;
using System;

namespace InteractionSystem
{
    public sealed partial class Sequence
    {
        public class SequenceMaster
        {
            private Sequence sequence;
            public SequenceMaster(Sequence sequence) =>
                this.sequence = sequence;

            public BaseInteractionAction FirstAction { get => sequence.FirstAction; set => sequence.FirstAction = value; }
            public List<BaseInteractionAction> Sequences { get => sequence.Sequences; set => sequence.Sequences = value; }
            public GameObject Object { get => sequence.Object; set => sequence.Object = value; }
            public GameObject Subject { get => sequence.Subject; set => sequence.Subject = value; }

            public Sequence Append(BaseInteractionAction sequence)
            {
                if (!Sequences.Contains(sequence))
                    Sequences.Add(sequence);

                return this.sequence;
            }
            public Sequence Remove(BaseInteractionAction sequence)
            {
                if (Sequences.Contains(sequence))
                    Sequences.Remove(sequence);

                return this.sequence;
            }
            public Sequence Remove(INode action)
            {
                if (action is BaseInteractionAction e)
                {
                    if (Sequences.Contains(e))
                        Sequences.Remove(e);
                }

                return this.sequence;
            }

            public IEnumerable<T> Get<T>() where T : BaseInteractionAction
            {
                foreach (var action in Sequences)
                    if (action is T)
                        yield return (T)action;
            }
            public IEnumerable<T> Get<T>(Func<T, bool> predicate) where T : BaseInteractionAction
            {
                foreach (var action in Sequences)
                    if (action is T typedAction)
                    {
                        if (predicate(typedAction))
                            yield return typedAction;
                    }
            }

            public void Clean()
            {
                if (sequence.IsProgress != false)
                {
                    Debug.LogError("Stoping of sequnce is inpossible when sequence in progress. Call StopSequence.");
                    return;
                }
                Sequences.ForEach(action => { action.OnExecutingEvent -= this.sequence.OnExecutingActionHandler; });
                Sequences.Clear();
                FirstAction = null;
            }
        }
    }
}
