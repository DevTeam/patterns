namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    using Platform.Reflection;

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

            yield return container.Bind<ISession, ITool, ExplorerTool>();
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<ISession, ITestSource, TestSource>();
        }
    }
}
