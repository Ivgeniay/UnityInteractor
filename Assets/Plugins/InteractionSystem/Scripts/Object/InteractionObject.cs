using UnityEngine;

namespace InteractionSystem
{
    public class InteractionObject : MonoBehaviour
    {
        
        [SerializeField] private Sequence Sequences;
        [SerializeField] private GameObject subject;

        private void Awake() => SetObject(this.gameObject); 

        public void AddAction(BaseInteractionAction interactionAction)
        {
            if (Sequences == null) Sequences = new Sequence();
            Sequences.Append(interactionAction);
        }
        public void RemoveAction(BaseInteractionAction interactionAction)
        {
            if (Sequences == null) Sequences = new Sequence();
            Sequences.Remove(interactionAction);
        }
        public void RemoveAction(INode interactionAction)
        {
            if (Sequences == null) Sequences = new Sequence();
            Sequences.Remove(interactionAction);
        }
        public void SetSubject(GameObject gameObject) => Sequences.SetSubject(gameObject);
        public void SetObject(GameObject gameObject) => Sequences.SetObject(gameObject);
        public void StartSequence()
        {
            if (Sequences.Subject == null) Sequences.Subject = subject;
            Sequences.StartSequence();
        }
        public void StopSequence()
        {
            if (Sequences.Subject == null) Sequences.Subject = subject;
            Sequences.StopSequence();
        }
        public void CleanSequence() => Sequences.Clean();


#if UNITY_EDITOR
        public Sequence GetSequence() => Sequences;
#endif

    }
}
