namespace DevTeam.TestTool.dotNet
{
    using System.Collections.Generic;

    using Engine.Host;

    using Engine.Contracts;

    using Patterns.IoC;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var container = new Container("root");
            using (new DotNetContainerConfiguration().Apply(container))
            using (new HostContainerConfiguration().Apply(container))
            {
                var commandLineArgsToPropertiesConverter = container.Resolve<IConverter<string[], IEnumerable<PropertyValue>>>();
                var properties = commandLineArgsToPropertiesConverter.Convert(args);
                var session = container.Resolve<IEnumerable<PropertyValue>, ISession>(properties);

                var toolFactory = container.Resolve<IContainer, IToolFactory>(container);

                using (toolFactory.Create(session, "runner").Run())
                using (toolFactory.Create(session, "explorer").Run())
                {
                }
            }
        }
    }
}