namespace DevTeam.TestTool.dotNetCore
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Engine.Contracts;
    using Engine.Host;

    public class DotNetCoreContainerConfiguration: IConfiguration
    {
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new HostContainerConfiguration();
        }

        public IEnumerable<IDisposable> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IOutput>(() => new Console(), nameof(Console));
        }
    }
}
