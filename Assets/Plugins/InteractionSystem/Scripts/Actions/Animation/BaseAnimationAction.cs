using System.Collections;
using UnityEngine; 
using System;
using System.ComponentModel;
using UnityEditor.Animations;

namespace InteractionSystem
{

    [Serializable] 
    public abstract class BaseAnimationAction : BaseInteractionAction, ISMCallback
    {
        [SerializeFieldNode]
        [SerializeField] 
        public string AnimationParameter = "IsRotate";
        protected Animator animator { get; set; }
        protected AnimatorController controller;
        private StateMachineCallbacks stateInvoker;

        private bool Inside = false;
        private int startAnimationHash = 0;
        private int nextAnimationHash = 0;

        public override void Awake()
        {
            animator = Performer.GetComponentInChildren<Animator>();
            controller = animator.runtimeAnimatorController as AnimatorController;
            stateInvoker = animator?.GetBehaviour<StateMachineCallbacks>();
            stateInvoker?.Register(this);
            startAnimationHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }
        protected override IEnumerator Procedure()
        {
            TriggerToStartAnim();
            yield return new WaitUntil(() => Inside == true);
            TriggerToStopAnim(); 
            yield return new WaitUntil(() => Inside == false);
            //yield return new WaitAnimatorStateChange(stateInvoker, startStateHash);
            //yield return new WaitEndAnimatorState(stateInvoker);
            //yield return new WaitAnimatorStateChange(stateInvoker, animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            Debug.Log($"Animation waiter ended from {animator.gameObject}");
        }

        protected abstract void TriggerToStartAnim();
        protected abstract void TriggerToStopAnim();

        public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        {
            int currentAnimationHash = animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash;
            int shortName = animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash; 
            bool haveParameter = CheckNeededParameter(shortName);
            
            if (animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == startAnimationHash && haveParameter)
            {
                Inside = true;
                nextAnimationHash = stateInfo.fullPathHash;
            }
        }
        
        public virtual void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        { 
            if (stateInfo.fullPathHash == nextAnimationHash && Inside)
            {
                Inside = false;
            }
        }
        public virtual void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        public override void Reset()
        {
            base.Reset();
            stateInvoker?.UnRegister(this);
        }

        private bool CheckNeededParameter(int stateHash)
        {
            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    if (state.state.nameHash != stateHash) continue;

                    foreach (AnimatorStateTransition transition in state.state.transitions)
                    {
                        foreach (AnimatorCondition condition in transition.conditions)
                        {
                            var haveParameter = condition.parameter == AnimationParameter;
                            if (haveParameter) return true;
                            //Debug.Log($"Condition: {state.state.name} {transition.name} {condition.parameter} {condition.mode} {condition.threshold}");
                        }
                    }
                }
            }
            return false;
        }

    }
}