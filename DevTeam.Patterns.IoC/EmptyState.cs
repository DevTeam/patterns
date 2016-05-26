namespace DevTeam.Patterns.IoC
{
    public class EmptyState
    {
        public static readonly EmptyState Shared = new EmptyState();

        private EmptyState()
        {
        }
    }
}