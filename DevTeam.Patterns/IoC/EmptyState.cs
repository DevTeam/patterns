namespace DevTeam.Patterns.IoC
{
    /// <summary>
    /// Represents empty state or value.
    /// </summary>
    public struct EmptyState
    {
        /// <summary>
        /// Provides the default shared contract.
        /// </summary>
        public static readonly EmptyState Shared = new EmptyState();
    }
}