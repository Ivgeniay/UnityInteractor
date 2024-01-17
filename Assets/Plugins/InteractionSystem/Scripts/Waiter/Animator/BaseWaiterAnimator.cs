using UnityEngine;

namespace InteractionSystem.Waiter
{
    public abstract class BaseWaiterAnimator : CustomYieldInstruction
    {
        public bool InProgress { get; protected set; } = true;
        protected int layer;
        protected readonly StateMachineCallbacks smCb;

        public BaseWaiterAnimator(StateMachineCallbacks smCb, int layer = 0)
        {
            if (smCb == null)
            {
                InProgress = false;
                Debug.LogError($"WaiterError");
            }
            else
            {
                this.smCb = smCb;
                smCb.OnSMEvent += OnSMHandler;
                this.layer = layer;
            }
        }

        protected abstract void OnSMHandler(StateMachineCallbacks.SMBehaviour type, Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
        public override bool keepWaiting { get => InProgress; }
        public override void Reset()
        {
            base.Reset();
            smCb.OnSMEvent -= OnSMHandler;
        }
    }
}
