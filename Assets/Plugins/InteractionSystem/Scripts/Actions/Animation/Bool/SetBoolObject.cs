﻿using UnityEngine;
using System; 

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода усправляет аниматором интерактивного объекта. Во время вызова она присваивает AnimationParameter булевый флаг в значение SettedValue. Ждет начало анимации, затем возвращает значение в !SettedValue. Затем ждет конца анимации. В момент конца анимации процедура считается выполненной.")]
    public class SetBoolObject : BaseAnimationAction
    {
        [SerializeField]
        [BoolFieldContext]
        protected bool settedValue;

        public override void Awake() =>
            animator = Object.GetComponent<Animator>();

        protected override void StartAnimation() =>
            animator.SetBool(AnimationParameter, settedValue);

        protected override void StopAnimation()
        {
            animator.SetBool(AnimationParameter, !settedValue);
        }
    }
}
