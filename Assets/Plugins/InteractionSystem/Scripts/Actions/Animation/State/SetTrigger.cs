using UnityEngine;

namespace InteractionSystem
{
    public class SetTrigger : BaseAnimationAction
    {
        protected override void StartAnim() => animator.SetTrigger(AnimationParameter);
        protected override void StopAnim() => animator.ResetTrigger(AnimationParameter);
    }
}

