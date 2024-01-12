using System.Collections.Generic; 
using UnityEngine;

namespace InteractionSystem
{
    public class AnimationTrackerManager : MonoBehaviour
    {
        public static AnimationTrackerManager instance;
        public static AnimationTrackerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<AnimationTrackerManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("[ANIMATION_TRACKER]");
                        instance = go.AddComponent<AnimationTrackerManager>();
                    }
                }
                return instance;
            }
        }

        private Dictionary<BaseAnimationAction, AnimationTracker> map = new();

        private void Update()
        {
            if (map.Count == 0) return;
            foreach (AnimationTracker tracker in map.Values)
                tracker.Update();
        }

        public AnimationTracker GetTracker(BaseAnimationAction animationAction)
        {
            if (map.TryGetValue(animationAction, out AnimationTracker tracker)) return tracker;

            animationAction.OnCompleteEvent += OnSequenceComplete;
            AnimationTracker tracker1 = new AnimationTracker
                (
                    animationAction.Animator,
                    animationAction.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name
                );
            map.Add(animationAction, tracker1);
            return tracker1;
        }

        private void OnSequenceComplete(BaseInteractionAction obj)
        {
            if (obj is BaseAnimationAction animationAction)
            {
                if (map.TryGetValue(animationAction, out AnimationTracker tracker))
                {
                    animationAction.OnCompleteEvent -= OnSequenceComplete;
                    map.Remove(animationAction);
                } 
            }
        }
    }
}
