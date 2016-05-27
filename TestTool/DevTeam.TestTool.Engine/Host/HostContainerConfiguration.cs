namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    using DevTeam.Patterns.Reactive;

    using Patterns.EventAggregator;

    public class HostContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container = new ReactiveContainerConfiguration().Apply(container);
            container = container.Resolve<IContainer>(nameof(HostContainerConfiguration));

            container.Register<IEnumerable<PropertyValue>, ISession>(p => new Session(p));
            container.Register<IContainer, IToolFactory>(p => new ToolFactory(p));

            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register<IConfiguration>(() => new Explorer.ExplorerContainerConfiguration(), WellknownTool.Explorer);
            
            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register<IConfiguration>(() => new Runner.RunnerContainerConfiguration(), WellknownTool.Runnner);

            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register<IEventAggregator>(() => new Aggregator());

            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register<IConverter<string[], IEnumerable<PropertyValue>>>(() => new CommandLineArgsToPropertiesConverter());

            return container;
        }
    }
}
