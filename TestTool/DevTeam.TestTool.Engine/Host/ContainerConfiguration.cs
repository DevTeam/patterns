namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    using Patterns.EventAggregator;

    public class ContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var eventAggregator = new Lazy<IEventAggregator>(() => new Aggregator());
            var commandLineArgsToPropertiesConverter = new Lazy<IConverter<string[], IEnumerable<PropertyValue>>>(() => new CommandLineArgsToPropertiesConverter());
            var explorerContainerConfiguration = new Lazy<IConfiguration>(() => new Explorer.ContainerConfiguration());
            var runnerContainerConfiguration = new Lazy<IConfiguration>(() => new Runner.ContainerConfiguration());
            var assemblyLoader = new Lazy<IAssemblyLoader>(() => new AssemblyLoader());
            var typeLoader = new Lazy<ITypeLoader>(() => new TypeLoader());
            var methodInfoLoader = new Lazy<IMethodInfoLoader>(() => new MethodInfoLoader());
            var instanceFactory = new Lazy<IInstanceFactory>(() => new InstanceFactory());

            container
                .Register(() => explorerContainerConfiguration.Value, "explorer")
                .Register(() => runnerContainerConfiguration.Value, "runner")
                .Register(() => eventAggregator.Value)
                .Register(() => commandLineArgsToPropertiesConverter.Value)
                .Register(() => assemblyLoader.Value)
                .Register(() => typeLoader.Value)
                .Register(() => methodInfoLoader.Value)
                .Register(() => instanceFactory.Value)
                .Register<IEnumerable<PropertyValue>, ISession>(p => new Session(p))
                .Register<IContainer, IToolFactory>(p => new ToolFactory(p));

            return container;
        }
    }
}
