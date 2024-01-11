using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    public class SubjectAnimationAction : BaseAnimationAction
    {
        public override void Awake()
        {
            animator = Subject.GetComponent<Animator>();
        }
    }
}