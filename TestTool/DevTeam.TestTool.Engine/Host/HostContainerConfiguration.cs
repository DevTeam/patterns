namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;
    using Explorer;
    using Patterns.Dispose;
    using Patterns.Reactive;
    using Patterns.EventAggregator;

    using Platform.Reflection;

    using Publisher;

    using Reporter;
    using Runner;

    public class HostContainerConfiguration: IConfiguration
    {
        public static readonly IConfiguration Shared = new HostContainerConfiguration();

        private HostContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return ReactiveContainerConfiguration.Shared;
            yield return EventAggregatorContainerConfiguration.Shared;
            yield return ReflectionContainerConfiguration.Shared;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Register<IEnumerable<IPropertyValue>, ISession>(properties => new Session(container.Resolver<ISession, ITool>(), properties));
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IPropertyFactory>(() => new PropertyFactory(container.ResolveAll<IProperty>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConverter<string[], IEnumerable<IPropertyValue>>>(() => new CommandLineArgsToPropertiesConverter(container.Resolve<IPropertyFactory>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IResolver<ISession, ITool>>(() => new ToolResolver(container.Resolver<IContainer>(), container.Resolver<IConfiguration>()));

            // Tools
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => ExplorerContainerConfiguration.Shared, WellknownTool.Explorer);
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => RunnerContainerConfiguration.Shared, WellknownTool.Runnner);
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => ReporterContainerConfiguration.Shared, WellknownTool.Reporter);
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => PublisherContainerConfiguration.Shared, WellknownTool.Publisher);

            // Properties
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => ToolProperty.Shared, ToolProperty.Shared.Id);
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => AssemblyProperty.Shared, AssemblyProperty.Shared.Id);
        }
    }
}
