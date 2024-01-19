using InteractionSystem.Repositories;
using UnityEngine;

namespace InteractionSystem
{
    internal class SequencesManager : MonoBehaviour
    {
        private static SequencesManager instance;
        public static SequencesManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SequencesManager>();
                    if (instance != null )
                    {
                        var go = new GameObject("[SEQUENCE_MANAGER]");
                        instance = go.AddComponent<SequencesManager>();
                    }
                }
                return instance;
            }
        }

        public Repository<InteractionObject> repository = new Repository<InteractionObject>();
    }
}
