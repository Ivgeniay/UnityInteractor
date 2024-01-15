using System;
using System.Collections;
using UnityEngine;

namespace InteractionSystem
{
    [Serializable]
    [Description(@"Эта нода хранит позицию позицию.")]
    public class VectorThree : BasePrimitivesAction
    {
        [SerializeField]
        [SerializeFieldNode]
        private Vector3 m_Position;


        public Vector3 ValuePosition { get => m_Position; }
        public override void Awake() { }
        protected override IEnumerator Procedure() { yield return null; }
    }
}
