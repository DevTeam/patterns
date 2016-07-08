﻿namespace DevTeam.TestTool.dotNet
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Engine.Contracts;

    using Patterns.IoC;

    using Container = Patterns.IoC.Container;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var container = new Container();
            using (new DotNetContainerConfiguration().Apply(container))
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