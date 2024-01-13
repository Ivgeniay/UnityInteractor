using System.Collections; 
using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    public abstract class BaseInteractionAction : INode
    {
        public event Action<BaseInteractionAction, bool> IsExecutingEvent;
        [field: SerializeField][HideInInspector] public string Name { get; set; }
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

        [SerializeReference] public BaseInteractionAction NextIAction;
        [SerializeReference] public BaseInteractionAction ReferenceAction;
        [SerializeReference] public BaseInteractionAction ParallelAction;

        [NonSerialized][HideInInspector] public GameObject Performer;
        [SerializeField][EnumFieldContext] public PerformerType PerformerType;

        public bool IsCompleted { get; protected set; } = false;

        protected CoroutineDisposer coroutine { get => CoroutineDisposer.Instance; }
        protected Coroutine parallel = null;
        private Action onCompleteCallback;

        public BaseInteractionAction()
        {
            if (string.IsNullOrEmpty(ID)) ID = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(Name)) Name = GetType().Name;
            Position = new Vector2(350, 200);
        }

        #region MainBehaviour
        public abstract void Awake();
        public IEnumerator MainProcedure()
        {
            IsExecutingEvent?.Invoke(this, true);
            if (ParallelAction != null) parallel = coroutine.StartC(ParallelAction.Procedure());
            yield return Procedure();
            //if (ParallelAction != null) yield return WaitFor(ParallelAction);
            if (ParallelAction != null) yield return WaitFor(parallel);
            yield return Complete();
        }
        protected abstract IEnumerator Procedure();
        protected virtual IEnumerator WaitFor(BaseInteractionAction action)
        {
            if (action != null)
                yield return new WaitUntil(() => action.IsCompleted);
            else yield return null;
        }
        protected virtual IEnumerator WaitFor(Coroutine cor)
        {
            yield return new WaitUntil(() => cor != null);
        }

        protected IEnumerator Complete()
        {
            IsCompleted = true;
            onCompleteCallback?.Invoke();
            IsExecutingEvent?.Invoke(this, false);
            if (NextIAction != null)
                yield return NextIAction.MainProcedure();
            else yield return null;
        }
        public virtual void Reset()
        {
            onCompleteCallback = null;
            IsCompleted = false;
        }
        #endregion

        #region SetUpAction
        public virtual BaseInteractionAction OnComplete(Action onCompleteCallback)
        {
            this.onCompleteCallback = onCompleteCallback;
            return this;
        }
        public virtual BaseInteractionAction NextAction(BaseInteractionAction iAction)
        {
            NextIAction = iAction;
            return this;
        }
        #endregion

    }

    public enum PerformerType
    {
        Object,
        Subject
    }
}
