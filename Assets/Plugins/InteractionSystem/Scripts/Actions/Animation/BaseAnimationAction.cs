using System.Collections; 
using UnityEditor.Animations; 
using UnityEngine; 
using System;

namespace InteractionSystem
{
    [Serializable]
    public abstract class BaseAnimationAction : BaseInteractionAction
    {
        [StringFieldContext]
        [SerializeField] public string AnimationParameter = "IsRotate";
        public Animator Animator { get; protected set; }

        protected AnimatorController animatorController;
        protected float normAnimation { get; set; } = 0;

        private Coroutine parallel = null;

        public override void Awake()
        {
            Animator = Object.GetComponent<Animator>();
        }

        public override IEnumerator Procedure()
        {
            if (ParallelAction != null) 
                parallel = coroutine.StartC(ParallelAction.Procedure());

            Animator.SetBool(AnimationParameter, true);

            yield return new WaitUntil(() => Animator.GetCurrentAnimatorClipInfo(0).Length > 0);

                var t = Animator.GetCurrentAnimatorClipInfo(0);
                var y = t[0];
                var u = y.clip;
                var name = u.name;
                Debug.Log($"{name}");

            yield return new WaitUntil(() => Animator.GetBool(AnimationParameter));
            yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length + 1);

            Animator.SetBool(AnimationParameter, false);

            yield return WaitFor(parallel);
            Debug.Log($"Анимация завершена! from object: {Object} subject: {Subject} with {GetType().Name}");
            yield return Complete();
        }

        public override void Reset()
        {
            base.Reset();
            normAnimation = 0;
        }

        bool IsAnimationComplete()
        {
            AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("YourAnimationName") && stateInfo.normalizedTime >= 1.0f;
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