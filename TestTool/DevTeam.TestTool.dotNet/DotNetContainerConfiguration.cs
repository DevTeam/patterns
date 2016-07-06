namespace DevTeam.TestTool.dotNet
{
    using System;
    using System.Collections.Generic;

    using Engine.Contracts;
    using Engine.Host;

    using Patterns.IoC;

    public class DotNetContainerConfiguration: IConfiguration
    {
        public static readonly IConfiguration Shared = new DotNetContainerConfiguration();

        private DotNetContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return HostContainerConfiguration.Shared;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IOutput>(() => new Console(), nameof(Console));
        }
    }
}
