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

            yield return container.Register<Session>().As<IEnumerable<IPropertyValue>, ISession>();
            yield return container.Register<PropertyFactory>(WellknownLifetime.Singleton).As<IPropertyFactory>();
            yield return container.Register<CommandLineArgsToPropertiesConverter>(WellknownLifetime.Singleton).As<IConverter<string[], IEnumerable<IPropertyValue>>>();
            yield return container.Register<ToolResolver>(WellknownLifetime.Singleton).As<IResolver<ISession, ITool>>();
            yield return container.Register<PropertyValue>().As<PropertyValueDescription, IPropertyValue>();

            // Tools
            yield return container.Register<ExplorerContainerConfiguration>(WellknownLifetime.Singleton).As<IConfiguration>(WellknownTool.Explorer);
            yield return container.Register<RunnerContainerConfiguration>(WellknownLifetime.Singleton).As<IConfiguration>(WellknownTool.Runnner);
            yield return container.Register<ReporterContainerConfiguration>(WellknownLifetime.Singleton).As<IConfiguration>(WellknownTool.Reporter);
            yield return container.Register<PublisherContainerConfiguration>(WellknownLifetime.Singleton).As<IConfiguration>(WellknownTool.Publisher);

            // Properties
            yield return container.Register<ToolProperty>(WellknownLifetime.Singleton).As<IProperty>(WellknownProperty.Tool);
            yield return container.Register<AssemblyProperty>(WellknownLifetime.Singleton).As<IProperty>(WellknownProperty.Assembly);
        }
    }
}
