namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;
    using Explorer;

    using Patterns.Reactive;
    using Patterns.EventAggregator;

    using Platform.Reflection;

    using Publisher;

    using Reporter;
    using Runner;

    /// <inheritdoc/>
    public class HostContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new ReactiveContainerConfiguration();
            yield return new EventAggregatorContainerConfiguration();
            yield return new ReflectionContainerConfiguration();
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Register<IEnumerable<IPropertyValue>, ISession>(properties => new Session(container.Resolver<ISession, ITool>(), properties, container.Resolve<IProperty>(WellknownProperty.Tool)));
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IPropertyFactory>(() => new PropertyFactory(container.ResolveAll<IProperty>(), container.Resolver<PropertyValueDescription, IPropertyValue>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConverter<string[], IEnumerable<IPropertyValue>>>(() => new CommandLineArgsToPropertiesConverter(container.Resolve<IPropertyFactory>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IResolver<ISession, ITool>>(() => new ToolResolver(container.Resolver<IContainer>(), container.Resolver<IConfiguration>()));
            yield return container.Register<PropertyValueDescription, IPropertyValue>(description => new PropertyValue(description));

            // Tools
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConfiguration>(() => new ExplorerContainerConfiguration(), WellknownTool.Explorer);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConfiguration>(() => new RunnerContainerConfiguration(), WellknownTool.Runnner);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConfiguration>(() => new ReporterContainerConfiguration(), WellknownTool.Reporter);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConfiguration>(() => new PublisherContainerConfiguration(), WellknownTool.Publisher);

            // Properties
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IProperty>(() => new ToolProperty(), WellknownProperty.Tool);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IProperty>(() => new AssemblyProperty(), WellknownProperty.Assembly);
        }
    }
}
