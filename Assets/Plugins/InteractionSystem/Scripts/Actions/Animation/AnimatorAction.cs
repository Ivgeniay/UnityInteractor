using System;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

namespace InteractionSystem
{
    [Serializable]
    public class AnimatorAction : BaseAnimationAction
    {
        private Animator animator;
        private AnimatorController animatorController;
        [SerializeField] public string RotateString = "IsRotate";
         
        private float normAnimation = 0;

        public override void Awake()
        {
            animator = Object.GetComponent<Animator>();
        }

        public override IEnumerator Procedure()
        {
            animator.SetBool(RotateString, true);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            //yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1f);
            //yield return new WaitUntil(() =>
            //    {
            //        float normTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            //        Debug.Log(normTime);
            //        if (normTime >= 0.99f || normTime < normAnimation) return true;
            //        else
            //        {
            //            normAnimation = normTime;
            //            return false;
            //        }
            //    });
            //yield return new WaitForSeconds(0.2f);
            animator.SetBool(RotateString, false);
            Debug.Log("Анимация завершена!");
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
