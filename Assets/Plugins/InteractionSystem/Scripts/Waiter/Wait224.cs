using UnityEngine;

namespace InteractionSystem
{
    internal class Wait224 : CustomYieldInstruction, ISMCallback
    {
        private int layerIndex;
        StateMachineCallbacks smCb;
        public Wait224(StateMachineCallbacks smCb, Animator animator, int layerIndex = 0)
        {
            this.layerIndex = layerIndex;
            this.smCb = smCb;
            this.smCb.Register(this);
        }

        ~Wait224()
        {
            this.smCb.UnRegister(this);
        }
        public override void Reset()
        {
            base.Reset();
            this.smCb.UnRegister(this);
        } 
        public override bool keepWaiting { get => false; }


        public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        {
        }

        public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}
