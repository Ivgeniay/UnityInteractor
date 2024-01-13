using System;
using System.Collections;

namespace InteractionSystem
{
    internal class FirstTest : BaseInteractionAction
    {
        public override void Awake()
        { 
        }

        protected override IEnumerator Procedure()
        {
            yield return null;
        }
    }
}
