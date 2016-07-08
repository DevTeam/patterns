namespace DevTeam.Patterns.IoC
{
    public enum WellknownLifetime
    {
        /// <summary>
        /// Self controlled.
        /// </summary>
        Transient,

        /// <summary>
        /// Shared for all resolves and self controlled.
        /// </summary>
        Singleton,

        /// <summary>
        /// Disposable instances will be disposed if a regestry is removed or a container is destroed.
        /// </summary>
        Controlled,

        /// <summary>
        /// Shared for all resolves. Disposable instances will be disposed if a regestry is removed or a container is destroed.
        /// </summary>
        ControlledSingleton,

        /// <summary>
        /// Shared for all resolves within resolving container and self controlled.
        /// </summary>
        ContainerSingleton,

        /// <summary>
        /// Shared for all resolves within resolving container and self controlled. Disposable instances will be disposed if a regestry is removed or a container is destroed.
        /// </summary>
        ControlledContainerSingleton
    }
}
