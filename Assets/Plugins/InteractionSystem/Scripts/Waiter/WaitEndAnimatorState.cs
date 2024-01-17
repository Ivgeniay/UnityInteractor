using InteractionSystem.Waiter;
using UnityEngine;

namespace InteractionSystem
{
    public sealed class WaitEndAnimatorState : BaseWaiterAnimator
    {
        private int starAnimationHash;
        private float startNormalizeTime;

        public WaitEndAnimatorState(StateMachineCallbacks smCb, Animator animator, int layer = 0) : base(smCb)
        {
            this.starAnimationHash = animator.GetCurrentAnimatorStateInfo(layer).fullPathHash;
            this.startNormalizeTime = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }

        protected override void OnSMHandler(StateMachineCallbacks.SMBehaviour type, Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (layerIndex != layer) return;
             
            switch (type)
            {
                case StateMachineCallbacks.SMBehaviour.Exit:

                    if (stateInfo.fullPathHash == starAnimationHash)
                        Finish();
                    
                    break;

                case StateMachineCallbacks.SMBehaviour.Update:

                    if (stateInfo.normalizedTime > startNormalizeTime && startNormalizeTime > 1) break;

                    var differentNormalizeTime = stateInfo.normalizedTime - startNormalizeTime;
                    if (stateInfo.fullPathHash == starAnimationHash && differentNormalizeTime >= 1)
                    {
                        Finish();
                        //Debug.Log(differentNormalizeTime);
                    }
                        //Finish();

                    break;
            }
        }

        private void Finish()
        {
            InProgress = false;
            smCb.OnSMEvent -= OnSMHandler;
        }

        public override void Reset()
        {
            base.Reset();
            smCb.OnSMEvent -= OnSMHandler;
        }
    }
}
