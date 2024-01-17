using System.Collections;
using UnityEngine; 
using System;
using System.ComponentModel;
using UnityEditor.Animations;

namespace InteractionSystem
{
    [Serializable] 
    public abstract class BaseAnimationAction : BaseInteractionAction
    {
        [SerializeFieldNode]
        [SerializeField] 
        public string AnimationParameter = "Parameter Name";

        [SerializeFieldNode]
        [SerializeField]
        public int AnimationLayer = 0;

        protected Animator animator { get; set; }
        protected AnimatorController controller;
        private StateMachineCallbacks stateInvoker;

        private int startAnimationHash = 0;

        public override void Awake()
        {
            animator = Performer.GetComponentInChildren<Animator>();
            controller = animator.runtimeAnimatorController as AnimatorController;
            stateInvoker = animator?.GetBehaviour<StateMachineCallbacks>();
            if (stateInvoker == null)
            {
                AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
                stateInvoker = animatorController.layers[AnimationLayer].stateMachine.AddStateMachineBehaviour<StateMachineCallbacks>();
            }
            startAnimationHash = animator.GetCurrentAnimatorStateInfo(AnimationLayer).fullPathHash;
        }
        protected override IEnumerator Procedure()
        {
            StartAnim();
            yield return new WaitEndAnimatorState(stateInvoker, animator);
            StopAnim();
            var currentAnimationHash = animator.GetCurrentAnimatorStateInfo(AnimationLayer).fullPathHash;
            yield return new WaitEndAnimatorState(stateInvoker, animator);
        }

        protected abstract void StartAnim();
        protected abstract void StopAnim();

    }
}