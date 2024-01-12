using System;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

namespace InteractionSystem
{
    [Serializable]
    public abstract class BaseAnimationAction : BaseInteractionAction
    {
        protected Animator animator;
        protected AnimatorController animatorController;
        [SerializeField] public string AnimationParameter = "IsRotate";
        protected float normAnimation = 0;

        public override void Awake()
        {
            animator = Object.GetComponent<Animator>();
        }

        public override IEnumerator Procedure()
        {
            animator.SetBool(AnimationParameter, true);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool(AnimationParameter, false);
            Debug.Log($"Анимация завершена! from object: {Object} subject: {Subject} with {GetType().Name}");
            yield return Complete();
        }

        public override void Reset()
        {
            base.Reset();
            normAnimation = 0;
        }

        public void Method()
        {
            animatorController = GetAnimatorController(animator);

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
