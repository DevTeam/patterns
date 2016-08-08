namespace DevTeam.TestTool.dotNetCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Patterns.IoC;
    using Patterns.EventAggregator;
    using Patterns.IoC.Configuration;
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
            yield return new ConfigurationsContainerConfiguration();
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var configFile = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "TestToolContainerConfiguration.json", SearchOption.AllDirectories).First();
            return container.Resolve<string, IConfiguration>(File.ReadAllText(configFile), WellknownConfigurations.Json).CreateRegistrations(container);
        }
    }
}
