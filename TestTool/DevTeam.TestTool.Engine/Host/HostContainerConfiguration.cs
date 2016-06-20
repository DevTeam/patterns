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
            disposable.Add(container.Register<IEnumerable<PropertyValue>, ISession>(p => new Session(p)));
            disposable.Add(container.Register<IContainer, IToolFactory>(p => new ToolFactory(p)));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new Explorer.ExplorerContainerConfiguration(), WellknownTool.Explorer));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConfiguration>(() => new Runner.RunnerContainerConfiguration(), WellknownTool.Runnner));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEventAggregator>(() => new Aggregator()));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConverter<string[], IEnumerable<PropertyValue>>>(() => new CommandLineArgsToPropertiesConverter()));

            return disposable;
        }
    }
}
