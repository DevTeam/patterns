namespace DevTeam.TestTool.dotNetCore
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;

    using Engine.Host;

    using Platform.System;

    /// <inheritdoc/>
    public class DotNetCoreContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new SystemContainerConfiguration();
            yield return new HostContainerConfiguration();
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield break;
        }
    }
}
