namespace DevTeam.TestTool.dotNetCore
{
    using System.Collections.Generic;

    using Patterns.Dispose;
    using Patterns.IoC;
    using Engine.Contracts;

    public class Program
    {
        public static void Main(string[] args)
        {
            var container = Containers.Create();
            using (container.Apply(new DotNetCoreContainerConfiguration()).ToCompositeDisposable())
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