using UnityEngine;

namespace InteractionSystem
{
    public class SetTrigger : BaseAnimationAction
    {
        protected override void TriggerToStartAnim() => animator.SetTrigger(AnimationParameter);
        protected override void TriggerToStopAnim() => animator.ResetTrigger(AnimationParameter);
    }
}

