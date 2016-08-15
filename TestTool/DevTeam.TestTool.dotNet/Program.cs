namespace DevTeam.TestTool.dotNet
{
    using System.Collections.Generic;

    using Engine.Contracts;

    using Patterns.Dispose;
    using Patterns.IoC;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var container = Containers.Create();
            using (container.Apply(new DotNetContainerConfiguration()).ToCompositeDisposable())
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