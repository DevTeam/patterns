namespace DevTeam.TestTool.dotNet
{
    using System.Collections.Generic;
    using System.Linq;

    using DevTeam.TestTool.Engine.Host;

    using Engine.Contracts;

    using Patterns.IoC;
    using Engine;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var rootContainer = new ContainerConfiguration().Apply(new Container());
            var commandLineArgsToPropertiesConverter = rootContainer.Resolve<IConverter<string[], IEnumerable<PropertyValue>>>();
            var properties = commandLineArgsToPropertiesConverter.Convert(args);
            var session = rootContainer.Resolve<IEnumerable<PropertyValue>, ISession>(properties);
            var toolFactory = rootContainer.Resolve<IContainer, IToolFactory>(rootContainer);
            var tool = toolFactory.Create(session);
            tool.Run();
        }
    }
}