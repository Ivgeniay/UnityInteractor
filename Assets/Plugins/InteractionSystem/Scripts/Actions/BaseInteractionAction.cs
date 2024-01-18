using System.Collections; 
using UnityEngine;
using System;
using static InteractionSystem.Sequence;

namespace InteractionSystem
{
    [Serializable]
    public abstract class BaseInteractionAction : INode
    {
        public event Action<BaseInteractionAction, ActionExecutionType> OnExecutingEvent;
        public ActionExecutionType CurrentExecutionType { get; private set; }
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
        /// <summary>
        /// Method to initalize Action
        /// </summary>
        public virtual void Awake() => InvokeExecuted(ActionExecutionType.Awake);
        /// <summary>
        /// OnStop is called when a sequence is forced to stop
        /// </summary>
        public virtual void OnStop() => InvokeExecuted(ActionExecutionType.OnStop);
        /// <summary>
        /// Main public behaviour of Action
        /// </summary>
        /// <returns></returns>
        public IEnumerator MainProcedure()
        {
            InvokeExecuted(ActionExecutionType.Procedure);
            if (ParallelAction != null) parallel = coroutine.StartC(ParallelAction.MainProcedure());
            yield return Procedure();
            if (ParallelAction != null)
            {
                InvokeExecuted(ActionExecutionType.WaitParallel);
                yield return WaitFor(ParallelAction);
            }
            yield return Complete();
        }
        /// <summary>
        /// Main private behaviour of Action
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator Procedure();
        /// <summary>
        /// Method for waiting for connected parallel actions
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator WaitFor(BaseInteractionAction action)
        {
            if (action != null) 
                yield return new WaitUntil(() => action.IsCompleted == true);
        }
        /// <summary>
        /// Method called after all actions, including parallel ones, have been completed
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Complete()
        {
            IsCompleted = true;
            onCompleteCallback?.Invoke(this); 
            InvokeExecuted(ActionExecutionType.Complete);

            if (NextIAction != null)
                yield return NextIAction.MainProcedure();
        }

        /// <summary>
        /// Method of rolling back an action to its original state
        /// </summary>
        public void Reset()
        {
            onCompleteCallback = null;
            IsCompleted = false;
            InvokeExecuted(ActionExecutionType.Waiting);
        }
        #endregion

        private void InvokeExecuted(ActionExecutionType value)
        {
            IsExecuting = 
                value == ActionExecutionType.Awake || 
                value == ActionExecutionType.Procedure || 
                value == ActionExecutionType.Exit || 
                value == ActionExecutionType.WaitParallel;
            CurrentExecutionType = value;
            OnExecutingEvent?.Invoke(this, value);
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
