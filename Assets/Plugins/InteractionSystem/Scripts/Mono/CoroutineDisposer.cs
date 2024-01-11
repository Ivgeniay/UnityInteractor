using System.Collections;
using UnityEngine;

namespace InteractionSystem
{
    public class CoroutineDisposer : MonoBehaviour
    {
        public static CoroutineDisposer instance = null;
        public static CoroutineDisposer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CoroutineDisposer>(true);
                    if (instance == null)
                    {
                        GameObject go = new GameObject("[IO_COROUTINE]");
                        instance = go.AddComponent<CoroutineDisposer>();
                    }
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        public Coroutine StartC(IEnumerator enumerator) =>
            StartCoroutine(enumerator);
        
        public void StopC(Coroutine coroutine) => 
            StopCoroutine(coroutine);

        public void StopAllC() => StopAllCoroutines();

    }
}
