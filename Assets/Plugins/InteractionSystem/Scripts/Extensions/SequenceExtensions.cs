using System;
using System.Collections.Generic;

namespace InteractionSystem
{
    public static class SequenceExtensions
    {
        public static void Register(this InteractionObject obj) =>
            SequencesManager.Instance.Register(obj);
        
    }
}
