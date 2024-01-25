namespace InteractionSystem
{
    internal static class SequenceManagerExtensions
    {
        public static void Register(this SequencesManager source, InteractionObject obj) =>
            source.repository.Register(obj);
        
    }
}
