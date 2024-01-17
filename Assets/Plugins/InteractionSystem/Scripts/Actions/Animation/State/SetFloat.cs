using System;
using UnityEngine;

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода усправляет аниматором интерактивного объекта. Во время вызова она присваивает AnimationParameter булевый флаг в значение SettedValue. Ждет начало анимации, затем возвращает значение в !SettedValue. Затем ждет конца анимации. В момент конца анимации процедура считается выполненной.")]
    public class SetFloat : BaseAnimationAction
    {
        [SerializeField]
        [SerializeFieldNode]
        private float SettedValue;

        [SerializeField]
        [SerializeFieldNode]
        private float ReSettedValue;

        protected override void StartAnim() => animator.SetFloat(AnimationParameter, SettedValue);
        protected override void StopAnim() => animator.SetFloat(AnimationParameter, ReSettedValue);
    }
}
