namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;
    using Explorer;

    using Publisher;

    using Reporter;
    using Runner;

    /// <inheritdoc/>
    public class HostContainerConfiguration: IConfiguration
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

            yield return container.Bind<IEnumerable<IPropertyValue>, ISession, Session>();
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IPropertyFactory, PropertyFactory>();
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IConverter<string[], IEnumerable<IPropertyValue>>, CommandLineArgsToPropertiesConverter>();
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IResolver<ISession, ITool>, ToolResolver>();
            yield return container.Bind<PropertyValueDescription, IPropertyValue, PropertyValue>();

            // Tools
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IConfiguration, ExplorerContainerConfiguration>(WellknownTool.Explorer);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IConfiguration, RunnerContainerConfiguration>(WellknownTool.Runnner);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IConfiguration, ReporterContainerConfiguration>(WellknownTool.Reporter);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IConfiguration, PublisherContainerConfiguration>(WellknownTool.Publisher);

            // Properties
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IProperty, ToolProperty>(WellknownProperty.Tool);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IProperty, AssemblyProperty>(WellknownProperty.Assembly);
        }
    }
}
