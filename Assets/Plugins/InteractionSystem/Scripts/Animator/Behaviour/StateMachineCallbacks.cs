using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace InteractionSystem
{
    public class StateMachineCallbacks : StateMachineBehaviour
    {
        private List<ISMCallback> instancesForCallback = new();

        internal void Register(ISMCallback baseAnimationAction)
        {
            if (!instancesForCallback.Contains(baseAnimationAction))
                instancesForCallback.Add(baseAnimationAction);
        }

        internal void UnRegister(ISMCallback baseAnimationAction)
        {
            if (instancesForCallback.Contains(baseAnimationAction))
                instancesForCallback.Remove(baseAnimationAction);
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (instancesForCallback.Count < 1) return;
            base.OnStateEnter(animator, stateInfo, layerIndex);
            instancesForCallback.ForEach(action =>
            {
                action.OnStateEnter(animator, stateInfo, layerIndex);
            });
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (instancesForCallback.Count < 1) return;
            base.OnStateExit(animator, stateInfo, layerIndex);
            instancesForCallback.ForEach(action =>
            {
                action.OnStateExit(animator, stateInfo, layerIndex);
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (instancesForCallback.Count < 1) return;
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            instancesForCallback.ForEach(action =>
            {
                action.OnStateUpdate(animator, stateInfo, layerIndex);
            });
        } 
    }
}
