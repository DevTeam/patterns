namespace DevTeam.TestTool.Engine.Publisher
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;
    using Host;

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

            yield return container.Register<PublisherTool>().As<ISession, ITool>();
            yield return container.Register<ReportPublisher>().As<IReportPublisher>(WellknownLifetime.Singleton);
            yield return container.Register<ConsoleOutput>().As<IOutput>(WellknownLifetime.Singleton);
        }
    }
}
