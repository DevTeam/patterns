namespace DevTeam.TestTool.dotNet
{
    using System;
    using System.Collections.Generic;

    using Engine.Contracts;
    using Engine.Host;

    using Patterns.IoC;

    public class DotNetContainerConfiguration: IConfiguration
    {
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new HostContainerConfiguration();
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IOutput>(() => new Console(), nameof(Console));
        }
    }
}
