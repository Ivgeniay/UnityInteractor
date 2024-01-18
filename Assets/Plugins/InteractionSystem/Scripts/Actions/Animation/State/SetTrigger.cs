using System;

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода усправляет аниматором интерактивного объекта. Во время вызова она вызывает срабатывание триггера AnimationParameter. Ждет начало анимации, затем вызывает ресет триггера. Затем ждет конца анимации. В момент конца анимационно состояния вызывающего триггер процедура считается выполненной.")]
    public class SetTrigger : BaseAnimationAction
    {
        protected override void StartAnim() => animator.SetTrigger(AnimationParameter);
        protected override void StopAnim() => animator.ResetTrigger(AnimationParameter);
    }
}

