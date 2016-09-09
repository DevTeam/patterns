namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an abstraction of IoC container.
    /// </summary>
    public interface IContainer : IResolver, IRegistry, IDisposable
    {
        /// <summary>
        /// Gets key/name of container.
        /// </summary>
        object Key { get; }

        IEnumerable<IRegistration> GetRegistrations(IContainerContext containerContext);
    }
}