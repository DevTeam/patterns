namespace DevTeam.Patterns.IoC
{
    using System;

    /// <summary>
    /// Represents an abstraction of IoC container.
    /// </summary>
    public interface IContainer : IResolver, IRegistry, IDisposable
    {
        /// <summary>
        /// Gets key/name of container.
        /// </summary>
        object Key { get; }
    }
}