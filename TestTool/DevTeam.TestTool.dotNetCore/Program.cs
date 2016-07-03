namespace DevTeam.TestTool.dotNetCore
{
    using System.Collections.Generic;

    using Patterns.IoC;
    using Engine.Contracts;
    using Engine.Host;

    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new Container();
            using (new DotNetCoreContainerConfiguration().Apply(container))
            using (new HostContainerConfiguration().Apply(container))
            {
                var commandLineArgsToPropertiesConverter = container.Resolve<IConverter<string[], IEnumerable<IPropertyValue>>>();
                var properties = commandLineArgsToPropertiesConverter.Convert(args);
                using (container.Resolve<IEnumerable<IPropertyValue>, ISession>(properties))
                {
                }
            }
        }
    }
}
