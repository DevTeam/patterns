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
            container = container.Resolve<IContainer>(typeof(HostContainerConfiguration).Name);

            var eventAggregator = new Lazy<IEventAggregator>(() => new Aggregator());
            var commandLineArgsToPropertiesConverter = new Lazy<IConverter<string[], IEnumerable<PropertyValue>>>(() => new CommandLineArgsToPropertiesConverter());
            var explorerContainerConfiguration = new Lazy<IConfiguration>(() => new Explorer.ExplorerContainerConfiguration());
            var runnerContainerConfiguration = new Lazy<IConfiguration>(() => new Runner.RunnerContainerConfiguration());            
            
            container
                .Register(() => explorerContainerConfiguration.Value, "explorer")
                .Register(() => runnerContainerConfiguration.Value, "runner")
                .Register(() => eventAggregator.Value)                
                .Register(() => commandLineArgsToPropertiesConverter.Value)
                .Register<IEnumerable<PropertyValue>, ISession>(p => new Session(p))
                .Register<IContainer, IToolFactory>(p => new ToolFactory(p));

            return container;
        }
    }
}
