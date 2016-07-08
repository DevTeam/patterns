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
        /// Shared for all resolves for the one container and self controlled.
        /// </summary>
        PerContainer,

        /// <summary>
        /// Shared for all resolves for the one container and self controlled. Disposable instances will be disposed if a regestry is removed or a container is destroed.
        /// </summary>
        ControlledPerContainer,

        /// <summary>
        /// Shared for all resolves for the one resolving and self controlled.
        /// </summary>
        PerResolveLifetime,

        /// <summary>
        /// Shared for all resolves for the one resolving. Disposable instances will be disposed if a regestry is removed or a container is destroed.
        /// </summary>
        ControlledPerResolveLifetime,

        /// <summary>
        /// Shared for all resolves for the one thread and self controlled.
        /// </summary>
        PerThreadLifetime,

        /// <summary>
        /// Shared for all resolves for the one thread. Disposable instances will be disposed if a regestry is removed or a container is destroed.
        /// </summary>
        ControlledPerThreadLifetime
    }
}
