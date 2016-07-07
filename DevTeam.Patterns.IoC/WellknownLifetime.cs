namespace DevTeam.Patterns.IoC
{
    public static class WellknownLifetime
    {
        /// <summary>
        /// Self controlled
        /// </summary>
        public static readonly string Transient = "Transient";

        /// <summary>
        /// Shared for all resolves
        /// </summary>
        public static readonly string Singleton = "Singleton";

        /// <summary>
        /// Disposable instanced will be disposed if a regestry is removed or a container is destroed
        /// </summary>
        public static readonly string Controlled = "Controlled";
    }
}
