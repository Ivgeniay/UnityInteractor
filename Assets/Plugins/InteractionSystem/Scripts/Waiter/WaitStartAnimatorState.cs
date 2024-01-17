using InteractionSystem.Waiter;
using UnityEngine;

namespace InteractionSystem
{
    public class WaitStartAnimatorState : BaseWaiterAnimator
    { 
        private int hashWaitAnimation;
        public WaitStartAnimatorState(StateMachineCallbacks smCb, int hashWaitAnimation, int layerIndex = 0) : base(smCb, layerIndex)
        {
            this.hashWaitAnimation = hashWaitAnimation;
        }

        public WaitStartAnimatorState(StateMachineCallbacks smCb, Animator animator, int layerIndex = 0) : base(smCb, layerIndex)
        {
            if (animator == null)
            {
                InProgress = false;
                Debug.LogError($"WaiterError");
            }
        }
        protected override void OnSMHandler(StateMachineCallbacks.SMBehaviour type, Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (layerIndex != layer) return;
            switch (type)
            {
                case StateMachineCallbacks.SMBehaviour.Enter:
                    if (stateInfo.fullPathHash == hashWaitAnimation)
                    {
                        InProgress = false;
                        smCb.OnSMEvent -= OnSMHandler;
                    }
                    break;
            }
        }


        public override void Reset()
        {
            base.Reset();
        }

    }
}
