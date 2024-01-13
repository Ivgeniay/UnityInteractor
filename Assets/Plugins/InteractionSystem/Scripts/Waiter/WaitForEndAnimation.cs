using UnityEngine;

namespace InteractionSystem
{
    public sealed class WaitForEndAnimation : CustomYieldInstruction
    {
        private readonly Animator animator;
        private readonly int layerIndex;
        private readonly int startAnimationHash;
        private readonly float startNormalizedTime;

        public bool AnimationEnded { get; private set; }
        public bool AnimationChanged { get; private set; }

        public WaitForEndAnimation(Animator animator, int layerIndex = 0)
        {
            this.animator = animator;
            this.layerIndex = layerIndex;
            this.startNormalizedTime = animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
            this.startAnimationHash = animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash;

            this.AnimationEnded = false;
            this.AnimationChanged = false;

            var an = animator.GetCurrentAnimatorClipInfo(layerIndex);
        }

        public override bool keepWaiting
        {
            get
            {
                if (animator == null || !animator.gameObject.activeInHierarchy) return false;

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
                var currentHash = stateInfo.fullPathHash;

                if (stateInfo.normalizedTime > startNormalizedTime &&
                    stateInfo.fullPathHash != startAnimationHash &&
                    stateInfo.fullPathHash != currentHash)
                {
                    AnimationChanged = true;
                    return false;
                }
                 
                if (stateInfo.normalizedTime > 1f )
                {
                    AnimationEnded = true;
                    return false;
                }
                return true;
            }
        }

    }
}
