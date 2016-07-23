namespace DevTeam.TestTool.Engine.Reporter
{
    using System;
    using System.Collections.Generic;

    using Patterns.EventAggregator;
    using Patterns.IoC;
    using Contracts;

    using Patterns.Reactive;

    /// <inheritdoc/>
    internal class ReporterContainerConfiguration: IConfiguration
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

            yield return container.Bind<ISession, ITool, ReporterTool>();
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<ITestReporter, TextTestReporter>("text");
        }
    }
}
