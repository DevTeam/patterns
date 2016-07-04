namespace DevTeam.Patterns.IoC
{
    public struct EmptyState
    {
        public static readonly EmptyState Shared = new EmptyState();        
    }
}