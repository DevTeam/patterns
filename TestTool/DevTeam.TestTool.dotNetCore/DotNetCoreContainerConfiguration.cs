namespace DevTeam.TestTool.dotNetCore
{
    using System;
    using System.Collections.Generic;

    using Patterns.Dispose;
    using Patterns.IoC;
    using Engine.Contracts;
    using Engine.Host;

    public class DotNetCoreContainerConfiguration: IConfiguration
    {
        public static readonly IConfiguration Shared = new DotNetCoreContainerConfiguration();

        private DotNetCoreContainerConfiguration()
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
