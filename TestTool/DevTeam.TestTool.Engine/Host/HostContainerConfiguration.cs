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

    public class HostContainerConfiguration: IConfiguration
    {
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new ReactiveContainerConfiguration();
            yield return new EventAggregatorContainerConfiguration();
            yield return new ReflectionContainerConfiguration();
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Register<IEnumerable<IPropertyValue>, ISession>(properties => new Session(container.Resolver<ISession, ITool>(), properties));
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IPropertyFactory>(() => new PropertyFactory(container.ResolveAll<IProperty>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConverter<string[], IEnumerable<IPropertyValue>>>(() => new CommandLineArgsToPropertiesConverter(container.Resolve<IPropertyFactory>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IResolver<ISession, ITool>>(() => new ToolResolver(container.Resolver<IContainer>(), container.Resolver<IConfiguration>()));

            // Tools
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConfiguration>(() => new ExplorerContainerConfiguration(), WellknownTool.Explorer);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConfiguration>(() => new RunnerContainerConfiguration(), WellknownTool.Runnner);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConfiguration>(() => new ReporterContainerConfiguration(), WellknownTool.Reporter);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConfiguration>(() => new PublisherContainerConfiguration(), WellknownTool.Publisher);

            // Properties
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register(() => ToolProperty.Shared, ToolProperty.Shared.Id);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register(() => AssemblyProperty.Shared, AssemblyProperty.Shared.Id);
        }
    }
}
