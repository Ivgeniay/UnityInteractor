using UnityEngine;

namespace InteractionSystem
{
    internal class Wait224 : CustomYieldInstruction
    {
        private int layerIndex;
        StateMachineCallbacks smCb;
        public Wait224(StateMachineCallbacks smCb, Animator animator, int layerIndex = 0)
        {
            this.layerIndex = layerIndex;
            this.smCb = smCb;
        }

        public override void Reset()
        {
            base.Reset(); 
        } 
        public override bool keepWaiting { get => false; }

    }
}
