namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Dispose;
    using Patterns.Reactive;

    using Patterns.EventAggregator;

    public class HostContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(new ReactiveContainerConfiguration().Apply(container));
            disposable.Add(new EventAggregatorContainerConfiguration().Apply(container));
            disposable.Add(container.Register<IEnumerable<PropertyValue>, ISession>(properties => new Session(container, container.Resolve<IEventAggregator>(), container.Resolve<IReportPublisher>(), properties)));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IPropertyFactory>(() => new PropertyFactory(container.ResolveAll<IProperty>())));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConverter<string[], IEnumerable<PropertyValue>>>(() => new CommandLineArgsToPropertiesConverter(container.Resolve<IPropertyFactory>())));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IReportPublisher>(() => new ReportPublisher(container.ResolveAll<IOutput>())));

            // Tools
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new Explorer.ExplorerContainerConfiguration(), WellknownTool.Explorer));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new Runner.RunnerContainerConfiguration(), WellknownTool.Runnner));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new Reporter.ReporterContainerConfiguration(), WellknownTool.Reporter));

            // Properties
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => ToolProperty.Shared, ToolProperty.Shared.Id));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => AssemblyProperty.Shared, AssemblyProperty.Shared.Id));

            return disposable;
        }
    }
}
