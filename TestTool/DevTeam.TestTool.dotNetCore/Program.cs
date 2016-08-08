namespace DevTeam.TestTool.dotNetCore
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Patterns.IoC;
    using Engine.Contracts;

    using Patterns.IoC.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new Container();
            //using (new DotNetCoreContainerConfiguration().Apply(container))
            //{
            //    var commandLineArgsToPropertiesConverter = container.Resolve<IConverter<string[], IEnumerable<IPropertyValue>>>();
            //    var properties = commandLineArgsToPropertiesConverter.Convert(args);
            //    using (container.Resolve<IEnumerable<IPropertyValue>, ISession>(properties))
            //    {
            //    }
            //}

            var configFile = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "TestToolContainerConfiguration.json", SearchOption.AllDirectories).First();

            using (new ConfigurationsContainerConfiguration().Apply(container))
            // Apply configuration
            using (container.Resolve<string, IConfiguration>(File.ReadAllText(configFile), WellknownConfigurations.Json).Apply(container))
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
