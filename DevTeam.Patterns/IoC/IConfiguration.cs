namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an abstraction for IoC configuration.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Returns dependencies for configuration.
        /// </summary>
        /// <returns>Dependencies.</returns>
        IEnumerable<IConfiguration> GetDependencies();

        /// <summary>
        /// Create registrations.
        /// </summary>
        /// <param name="container">The target container.</param>
        /// <returns>The created registrations.</returns>
        IEnumerable<IRegistration> CreateRegistrations(IContainer container);
	}
}