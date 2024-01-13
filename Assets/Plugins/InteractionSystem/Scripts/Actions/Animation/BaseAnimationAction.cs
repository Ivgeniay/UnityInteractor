using System.Collections; 
using UnityEditor.Animations; 
using UnityEngine; 
using System;
using System.Threading.Tasks;

namespace InteractionSystem
{
    [Serializable]
    public abstract class BaseAnimationAction : BaseInteractionAction
    {
        [StringFieldContext]
        [SerializeField] public string AnimationParameter = "IsRotate";
        public Animator Animator { get; protected set; }
        protected AnimatorController animatorController;

        protected override IEnumerator Procedure()
        {
            Animator.SetBool(AnimationParameter, true);
            yield return new WaitForStartAnimation(Animator);
            Animator.SetBool(AnimationParameter, false);
            yield return new WaitForEndAnimation(Animator);
            Debug.Log($"Animation waiter ended from {Animator.gameObject}");
            yield return Complete();
        }

        public void Method()
        {
            animatorController = GetAnimatorController(Animator);

            AnimatorControllerParameter[] parameters = animatorController.parameters;
            foreach (var parameter in parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Float)
                {
                    Debug.Log($"Name: {parameter.name}, Type: {parameter.type} default: {parameter.defaultFloat}");
                }
                else if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    Debug.Log($"Name: {parameter.name}, Type: {parameter.type} default: {parameter.defaultBool}");
                }
            }
        }
        private AnimatorController GetAnimatorController(Animator animator)
        {
            if (animator == null)
            {
                Debug.LogError("Animator is null");
                return null;
            }

            AnimatorController runtimeController = animator.runtimeAnimatorController as AnimatorController;
            if (runtimeController == null)
            {
                AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if (overrideController != null)
                {
                    runtimeController = overrideController.runtimeAnimatorController as AnimatorController;
                }
            }
            return runtimeController;
        }
    }
}