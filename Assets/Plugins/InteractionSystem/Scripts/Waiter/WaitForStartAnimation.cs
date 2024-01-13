using UnityEngine;

namespace InteractionSystem
{
    public class WaitForStartAnimation : CustomYieldInstruction
    {
        private Animator animator;
        private int layerIndex;
        private float startNormalizedTime;
        private float currentNormalizeTime;

        public WaitForStartAnimation(Animator animator, int layerIndex = 0)
        {
            this.animator = animator;
            this.layerIndex = layerIndex;
            this.startNormalizedTime = animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
            this.currentNormalizeTime = this.startNormalizedTime;
        }

        public override bool keepWaiting
        {
            get
            {
                currentNormalizeTime = animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
                bool result = currentNormalizeTime < startNormalizedTime && currentNormalizeTime < 1;
                return !result;
            }
        }
    }
}
