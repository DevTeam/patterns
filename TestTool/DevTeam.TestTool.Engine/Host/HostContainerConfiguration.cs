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
    using Publisher;

    using Reflection;

    using Reporter;
    using Runner;

    public class HostContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(new ReactiveContainerConfiguration().Apply(container));
            disposable.Add(new EventAggregatorContainerConfiguration().Apply(container));
            disposable.Add(new ReflectionContainerConfiguration().Apply(container));

            disposable.Add(container.Register<IEnumerable<IPropertyValue>, ISession>(properties => new Session(container.Resolver<ISession, ITool>(), properties)));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IPropertyFactory>(() => new PropertyFactory(container.ResolveAll<IProperty>())));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConverter<string[], IEnumerable<IPropertyValue>>>(() => new CommandLineArgsToPropertiesConverter(container.Resolve<IPropertyFactory>())));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IResolver<ISession, ITool>>(() => new ToolResolver(container.Resolver<IContainer>(), container.Resolver<IConfiguration>())));

            // Tools
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new ExplorerContainerConfiguration(), WellknownTool.Explorer));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new RunnerContainerConfiguration(), WellknownTool.Runnner));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new ReporterContainerConfiguration(), WellknownTool.Reporter));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new PublisherContainerConfiguration(), WellknownTool.Publisher));

            // Properties
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => ToolProperty.Shared, ToolProperty.Shared.Id));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => AssemblyProperty.Shared, AssemblyProperty.Shared.Id));

            return disposable;
        }
    }
}
