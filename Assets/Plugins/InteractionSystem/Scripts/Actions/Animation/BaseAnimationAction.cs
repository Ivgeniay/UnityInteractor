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
            //animator.SetBool(AnimationParameter, SettedValue);
            StartAnimation();
            yield return new WaitForStartAnimation(animator);
            StopAnimation();
            //animator.SetBool(AnimationParameter, !SettedValue);
            yield return new WaitForEndAnimation(animator);
            Debug.Log($"Animation waiter ended from {animator.gameObject}");
            yield return Complete();
        }

        protected abstract void StartAnimation();
        protected abstract void StopAnimation();
    }
}