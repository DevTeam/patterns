using System;
using System.Collections.Generic;

namespace DevTeam.Platform.System
{
    using Patterns.IoC;    

    /// <inheritdoc/>
    public class SystemContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConsole>(() => new Console());            
        }
    }
}
