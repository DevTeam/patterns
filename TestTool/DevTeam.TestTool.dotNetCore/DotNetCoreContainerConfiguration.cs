namespace DevTeam.TestTool.dotNetCore
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;

    using Engine.Host;

    using Patterns.EventAggregator;
    using Patterns.Reactive;

    using Platform.Reflection;
    using Platform.System;

    /// <inheritdoc/>
    public class DotNetCoreContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new ReactiveContainerConfiguration();
            yield return new EventAggregatorContainerConfiguration();
            yield return new ReflectionContainerConfiguration();
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
