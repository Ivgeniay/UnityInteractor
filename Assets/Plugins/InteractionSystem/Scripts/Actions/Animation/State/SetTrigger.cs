using UnityEngine;

namespace InteractionSystem
{
    public class SetTrigger : BaseAnimationAction
    {
        public override void Awake()
        {
            animator = Performer.GetComponent<Animator>();
            base.Awake();
        }

        protected override void TriggerToStartAnim() => animator.SetTrigger(AnimationParameter);
        protected override void TriggerToStopAnim() => animator.ResetTrigger(AnimationParameter);
    }
}

