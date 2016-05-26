namespace DevTeam.Patterns.IoC
{
    internal class EmptyState
    {
        public static readonly EmptyState Shared = new EmptyState();

        private EmptyState()
        {
        }
    }
}