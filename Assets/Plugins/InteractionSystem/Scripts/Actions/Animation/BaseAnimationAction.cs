using System.Collections; 
using UnityEditor.Animations; 
using UnityEngine; 
using System;
using InteractionSystem;

namespace InteractionSystem
{

    [Serializable]
    public abstract class BaseAnimationAction : BaseInteractionAction
    {
        [StringFieldContext]
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