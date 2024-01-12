using UnityEngine;
using System; 

namespace InteractionSystem
{
    public class AnimationTracker
    {
        private Animator animator;
        private string animationName;
        private float previousNormalizedTime;

        public event Action<string> OnAnimationStart;
        public event Action<string> OnAnimationEnd;

        public AnimationTracker(Animator animator, string animationName)
        {
            this.animator = animator;
            this.animationName = animationName;
        }

        public void Update()
        {
            if (animator == null)
            {
                // Обработка ситуации, если аниматор не установлен
                return;
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float currentNormalizedTime = stateInfo.normalizedTime;

            if (stateInfo.IsName(animationName))
            {
                // Анимация начинается
                if (currentNormalizedTime > previousNormalizedTime && currentNormalizedTime <= 0.01f)
                {
                    OnAnimationStart?.Invoke(animationName);
                }

                // Анимация завершается
                if (currentNormalizedTime < previousNormalizedTime && currentNormalizedTime >= 0.99f)
                {
                    OnAnimationEnd?.Invoke(animationName);
                }
            }

            // Сохраняем текущее значение normalizedTime для следующего обновления
            previousNormalizedTime = currentNormalizedTime;
        }
    }
}
