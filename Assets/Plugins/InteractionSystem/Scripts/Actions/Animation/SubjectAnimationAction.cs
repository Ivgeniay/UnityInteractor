using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    public class SubjectAnimationAction : BaseAnimationAction
    {
        public override void Awake()
        {
            Animator = Subject.GetComponent<Animator>();
        }
    }
}