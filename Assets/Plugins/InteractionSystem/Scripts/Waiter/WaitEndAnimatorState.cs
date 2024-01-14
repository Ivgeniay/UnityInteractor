using UnityEngine;

namespace InteractionSystem
{
    public sealed class WaitEndAnimatorState : CustomYieldInstruction, ISMCallback
    {
        private bool InProgress = true;
        StateMachineCallbacks smCb;
        public WaitEndAnimatorState(StateMachineCallbacks smCb)
        {
            this.smCb = smCb;
            this.smCb.Register(this);
        }

        ~WaitEndAnimatorState()
        {
            this.smCb.UnRegister(this);
        }
        public override void Reset()
        {
            base.Reset();
            this.smCb.UnRegister(this);
        }


        public override bool keepWaiting
        {
            get
            {
                if (!InProgress) Reset();
                return InProgress;
            }
        }

        public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            InProgress = false;
        }

        public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        { 
            //if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) InProgress = false;
        }
    }
}
