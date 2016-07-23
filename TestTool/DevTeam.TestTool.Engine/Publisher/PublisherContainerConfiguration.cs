namespace DevTeam.TestTool.Engine.Publisher
{
    using System;
    using System.Collections.Generic;

    using Patterns.EventAggregator;
    using Patterns.IoC;
    using Contracts;
    using Host;

    using Platform.System;

    /// <inheritdoc/>
    internal class PublisherContainerConfiguration : IConfiguration
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

            yield return container.Bind<ISession, ITool, PublisherTool>();
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IReportPublisher, ReportPublisher>();
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IOutput, ConsoleOutput>();            
        }
    }
}
