using InteractionSystem.Waiter;
using UnityEngine;

namespace InteractionSystem
{
    public sealed class WaitEndAnimatorState : BaseWaiterAnimator
    {
        private int starAnimationHash;
        public WaitEndAnimatorState(StateMachineCallbacks smCb, int starAnimationFullpathHash, int layer = 0) : base(smCb, layer)
        {
            this.starAnimationHash = starAnimationFullpathHash;
        }
        public WaitEndAnimatorState(StateMachineCallbacks smCb, Animator animator, int layer = 0) : base(smCb)
        {
            this.starAnimationHash = animator.GetCurrentAnimatorStateInfo(layer).fullPathHash;
        }

        protected override void OnSMHandler(StateMachineCallbacks.SMBehaviour type, Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (layerIndex != layer) return;
            switch (type)
            {
                case StateMachineCallbacks.SMBehaviour.Exit:
                    if (stateInfo.fullPathHash == starAnimationHash)
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
            smCb.OnSMEvent -= OnSMHandler;
        }
    }
}
