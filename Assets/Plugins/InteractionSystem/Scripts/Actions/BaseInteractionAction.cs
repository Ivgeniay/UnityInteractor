using System.Collections; 
using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    public abstract class BaseInteractionAction : INode
    {
        public event Action<BaseInteractionAction, bool> OnExecutingEvent;
        public bool IsExecuting { get; private set; }

        [field: SerializeField][HideInInspector] public string Name { get; set; }
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

        [SerializeReference] public BaseInteractionAction NextIAction;
        [SerializeReference] public BaseInteractionAction ReferenceAction;
        [SerializeReference] public BaseInteractionAction ParallelAction;

        [NonSerialized][HideInInspector] public GameObject Performer;
        [SerializeField][SerializeFieldNode] public PerformerType PerformerType;
        public object[] InnerData { get; protected set; }
        public bool IsCompleted { get; protected set; } = false;

        protected CoroutineDisposer coroutine { get => CoroutineDisposer.Instance; }
        protected Coroutine parallel = null;
        private Action<BaseInteractionAction> onCompleteCallback;

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
            InvokeExecuted(true);
            //if (ParallelAction != null) parallel = coroutine.StartC(ParallelAction.Procedure());
            if (ParallelAction != null) parallel = coroutine.StartC(ParallelAction.MainProcedure());
            yield return Procedure();
            if (ParallelAction != null) yield return WaitFor(ParallelAction);
            yield return Complete();
        }
        protected abstract IEnumerator Procedure();
        protected virtual IEnumerator WaitFor(BaseInteractionAction action)
        {
            if (action != null) 
                yield return new WaitUntil(() => action.IsCompleted == true);
        }
        protected virtual IEnumerator WaitFor(Coroutine cor)
        {
            yield return new WaitUntil(() => cor != null);
        }
            //if (ParallelAction != null) yield return WaitFor(parallel);

        protected IEnumerator Complete()
        {
            IsCompleted = true;
            onCompleteCallback?.Invoke(this); 
            InvokeExecuted(false);

            if (NextIAction != null)
                yield return NextIAction.MainProcedure();
        }
        public virtual void Reset()
        {
            onCompleteCallback = null;
            IsCompleted = false;
        }
        #endregion

        private void InvokeExecuted(bool value)
        {
            IsExecuting = value;
            OnExecutingEvent?.Invoke(this, IsExecuting);
        }

        #region SetUpAction
        public virtual BaseInteractionAction OnComplete(Action<BaseInteractionAction> onCompleteCallback)
        {
            this.onCompleteCallback = onCompleteCallback;
            return this;
        }
        public virtual BaseInteractionAction SetNextAction(BaseInteractionAction iAction)
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
