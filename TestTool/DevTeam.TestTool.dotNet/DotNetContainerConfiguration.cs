namespace DevTeam.TestTool.dotNet
{
    using System;
    using System.Collections.Generic;

    using Engine.Host;

    using NUnit;

    using Patterns.EventAggregator;
    using Patterns.IoC;
    using Patterns.Reactive;

    using Platform.Reflection;
    using Platform.System;

    /// <inheritdoc/>
    public class DotNetContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new ReactiveContainerConfiguration();
            yield return new EventAggregatorContainerConfiguration();
            yield return new ReflectionContainerConfiguration();
            yield return new SystemContainerConfiguration();
            yield return new HostContainerConfiguration();
            yield return new NUnitContainerConfiguration();            
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield break;
        }
    }
}
