namespace DevTeam.TestTool.dotNetCore
{
    using System.Collections.Generic;

    using Patterns.IoC;
    using Engine.Contracts;

    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new Container();
            using (container.Apply(DotNetCoreContainerConfiguration.Shared))
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
