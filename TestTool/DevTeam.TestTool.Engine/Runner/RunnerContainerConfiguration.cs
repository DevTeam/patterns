namespace DevTeam.TestTool.Engine.Runner
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    /// <inheritdoc/>
    internal class RunnerContainerConfiguration: IConfiguration
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

            yield return container.Bind<ISession, ITool, RunnerTool>();

            yield return container
                .Using<ILifetime>(WellknownLifetime.Singleton)
                .Bind<ITestRunner, TestRunner>();
        }
    }
}
