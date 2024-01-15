﻿using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода усправляет аниматором интерактивного объекта. Во время вызова она присваивает AnimationParameter булевый флаг в значение SettedValue. Ждет начало анимации, затем возвращает значение в !SettedValue. Затем ждет конца анимации. В момент конца анимации процедура считается выполненной.")]
    public class SetBool : BaseAnimationAction
    {
        [SerializeField]
        [SerializeFieldNode]
        protected bool settedValue;

        protected override void TriggerToStartAnim() => animator.SetBool(AnimationParameter, settedValue);
        protected override void TriggerToStopAnim() => animator.SetBool(AnimationParameter, !settedValue);
        
    }
}
