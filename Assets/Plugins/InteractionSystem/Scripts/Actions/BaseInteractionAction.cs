using System.Collections; 
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace InteractionSystem
{
    [Serializable]
    public abstract class BaseInteractionAction : INode
    {
        [field: SerializeField][HideInInspector] public string Name { get; set; }
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

        [SerializeReference] public BaseInteractionAction NextIAction;
        [SerializeReference] public BaseInteractionAction ReferenceAction;
        [SerializeReference] public BaseInteractionAction ParallelAction;

        [NonSerialized][HideInInspector] public GameObject Object;
        [NonSerialized][HideInInspector] public GameObject Subject;

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
            if (ParallelAction != null) parallel = coroutine.StartC(ParallelAction.Procedure());
            yield return Procedure();
            yield return WaitFor(ParallelAction);
            yield return Complete();
        }
        protected abstract IEnumerator Procedure();
        protected virtual IEnumerator WaitFor(BaseInteractionAction baseInteractionAction)
        {
            if (baseInteractionAction != null) yield return new WaitUntil(() => baseInteractionAction.IsCompleted);
        }
        protected virtual IEnumerator WaitFor(Coroutine cor)
        {
            if (cor != null) yield return cor;
        }

        /// <summary>
        /// Must be used on end of procedure by yield return
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Complete()
        {
            IsCompleted = true;
            onCompleteCallback?.Invoke();
            if (NextIAction != null)
            {
                yield return NextIAction.MainProcedure();
            }
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
}
