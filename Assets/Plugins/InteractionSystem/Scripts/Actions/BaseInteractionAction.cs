using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

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
        private Action onCompleteCallback;

        public BaseInteractionAction()
        {
            if (string.IsNullOrEmpty(ID)) ID = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(Name)) Name = GetType().Name;
            Position = new Vector2(350, 200);
        }

        #region MainBehaviour
        public abstract void Awake();
        public abstract IEnumerator Procedure();

        public virtual IEnumerator WaitFor(BaseInteractionAction baseInteractionAction)
        {
            if (baseInteractionAction != null)
                yield return new WaitUntil(() => baseInteractionAction.IsCompleted);
        }

        /// <summary>
        /// Must be used on end of procedure by yield return
        /// </summary>
        /// <returns></returns>
        public IEnumerator Complete()
        {
            IsCompleted = true;
            onCompleteCallback?.Invoke();
            if (!IsCompleted) yield return null;
            if (NextIAction != null)
            {
                yield return NextIAction.Procedure();
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
