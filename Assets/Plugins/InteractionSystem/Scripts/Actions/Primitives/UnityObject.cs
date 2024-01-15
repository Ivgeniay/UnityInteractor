using System.Collections;
using UnityEngine;
using System;

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода хранит информацию об объекте на сцене позицию.")]
    public class UnityObject : BasePrimitivesAction
    {
        [SerializeField]
        [SerializeFieldNode(Description = "Используйте это поле только для объектов со сцены")]
        public GameObject Value;

        public override void Awake() { }

        protected override IEnumerator Procedure()
        {
            yield return null;
        }
    }
}
