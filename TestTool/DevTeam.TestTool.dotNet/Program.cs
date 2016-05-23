namespace DevTeam.TestTool.dotNet
{
    using System.Collections.Generic;

    using Engine.Contracts;

    using Patterns.IoC;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var rootContainer = new Engine.Host.ContainerConfiguration().Apply(new Container());
            var commandLineArgsToPropertiesConverter = rootContainer.Resolve<IConverter<string[], IEnumerable<PropertyValue>>>();
            var properties = commandLineArgsToPropertiesConverter.Convert(args);
            var session = rootContainer.Resolve<IEnumerable<PropertyValue>, ISession>(properties);
            var toolContainer = rootContainer.Resolve<IContainer>();
            var toolFactory = rootContainer.Resolve<IContainer, IToolFactory>(toolContainer);
            var tool = toolFactory.Create(session);
            tool.Run();
        }
    }
}