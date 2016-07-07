namespace DevTeam.Patterns.IoC
{
    public enum WellknownLifetime
    {
        /// <summary>
        /// Self controlled
        /// </summary>
        Transient,

        /// <summary>
        /// Shared for all resolves
        /// </summary>
        Singleton,

        /// <summary>
        /// Disposable instanced will be disposed if a regestry is removed or a container is destroed
        /// </summary>
        Controlled
    }
}
