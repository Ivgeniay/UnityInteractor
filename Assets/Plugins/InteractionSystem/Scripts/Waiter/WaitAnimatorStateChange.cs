using UnityEngine;

namespace InteractionSystem
{
    public class WaitAnimatorStateChange : CustomYieldInstruction, ISMCallback
    {
        private int layerIndex;

        private StateMachineCallbacks smCb;
        private int hashStartAnimation;
        private int hashNewAnimation;

        public WaitAnimatorStateChange(StateMachineCallbacks smCb, int hashStartAnimation, int layerIndex = 0)
        {
            this.layerIndex = layerIndex;
            this.smCb = smCb;
            this.smCb.Register(this);

            this.hashStartAnimation = hashStartAnimation;
            hashNewAnimation = this.hashStartAnimation;
        }

        public WaitAnimatorStateChange(StateMachineCallbacks smCb, Animator animator, int layerIndex = 0)
        {
            this.layerIndex = layerIndex;
            this.smCb = smCb;
            this.smCb.Register(this);

            hashStartAnimation = animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash;
            hashNewAnimation = hashStartAnimation;
        }

        public override bool keepWaiting
        {
            get
            {
                if (hashNewAnimation != hashStartAnimation) Reset();
                return hashNewAnimation == hashStartAnimation;
            }
        }

        ~WaitAnimatorStateChange()
        {
            smCb.UnRegister(this);
        }
        public override void Reset()
        {
            base.Reset();
            this.smCb.UnRegister(this);
        }

        public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            hashNewAnimation = stateInfo.fullPathHash;
        }

        public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }
}
