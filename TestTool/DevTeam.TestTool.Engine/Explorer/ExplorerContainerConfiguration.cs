namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    /// <inheritdoc/>
    internal class ExplorerContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Register<ExplorerTool>().As<ISession, ITool>();
            yield return container.Register<TestSource>(WellknownLifetime.Singleton).As<ISession, ITestSource>();
        }
    }
}
