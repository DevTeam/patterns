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
            var container = new Container();
            using (new DotNetContainerConfiguration().Apply(container))
            using (new HostContainerConfiguration().Apply(container))
            {
                var commandLineArgsToPropertiesConverter = container.Resolve<IConverter<string[], IEnumerable<PropertyValue>>>();
                var properties = commandLineArgsToPropertiesConverter.Convert(args);
                using (container.Resolve<IEnumerable<PropertyValue>, ISession>(properties))
                {                    
                }
            }
        }
    }
}