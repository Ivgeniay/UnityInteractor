using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода усправляет аниматором субъекта итерации. Во время вызова она присваивает AnimationParameter булевый флаг в значение SettedValue. Ждет начало анимации, затем возвращает значение в !SettedValue. Затем ждет конца анимации. В момент конца анимации процедура считается выполненной.")]
    public class SetBoolSubject : BaseAnimationAction
    {
        [SerializeField]
        [BoolFieldContext]
        protected bool settedValue;

        public override void Awake()
        {
            animator = Subject.GetComponent<Animator>();
        }

        protected override void StartAnimation()
        {
            animator.SetBool(AnimationParameter, settedValue);
        }

        protected override void StopAnimation()
        {
            animator.SetBool(AnimationParameter, !settedValue);
        }
    }
}