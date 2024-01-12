using UnityEngine;
using System; 

namespace InteractionSystem
{
    [Serializable]
    public class ObjectAnimatorAction : BaseAnimationAction
    {
        public override void Awake()
        {
            Animator = Object.GetComponent<Animator>();
        }

    }
}
