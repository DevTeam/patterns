namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    public class ContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            var commandLineArgsToPropertiesConverter = new Lazy<IConverter<string[], IEnumerable<PropertyValue>>>(() => new CommandLineArgsToPropertiesConverter());
            var explorerContainerConfiguration = new Lazy<IConfiguration>(() => new Explorer.ContainerConfiguration());
            var runnerContainerConfiguration = new Lazy<IConfiguration>(() => new Runner.ContainerConfiguration());
            container.Register(() => explorerContainerConfiguration.Value, "explorer")
                .Register(() => runnerContainerConfiguration.Value, "runner")
                .Register(() => commandLineArgsToPropertiesConverter.Value)
                .Register<IEnumerable<PropertyValue>, ISession>(p => new Session(p))
                .Register<IContainer, IToolFactory>(p => new ToolFactory(p));
            return container;
        }
    }
}
