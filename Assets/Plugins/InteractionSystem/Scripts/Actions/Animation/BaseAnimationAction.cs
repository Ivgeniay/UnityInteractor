using System.Collections;
using UnityEngine; 
using System;

namespace InteractionSystem
{

    [Serializable]
    public abstract class BaseAnimationAction : BaseInteractionAction
    {
        [SerializeFieldNode]
        [SerializeField] 
        public string AnimationParameter = "IsRotate";
        protected Animator animator { get; set; }

        protected override IEnumerator Procedure()
        {
            TriggerToStartAnim();
            yield return new WaitForStartAnimation(animator);
            TriggerToStopAnim();
            yield return new WaitForEndAnimation(animator);
            Debug.Log($"Animation waiter ended from {animator.gameObject}");
        }

        protected abstract void TriggerToStartAnim();
        protected abstract void TriggerToStopAnim();
    }
}