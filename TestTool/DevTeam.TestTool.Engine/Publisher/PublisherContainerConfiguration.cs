namespace DevTeam.TestTool.Engine.Publisher
{
    using System;
    using System.Collections.Generic;

    using Patterns.EventAggregator;
    using Patterns.IoC;
    using Contracts;
    using Host;    

    public class PublisherContainerConfiguration : IConfiguration
    {
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new EventAggregatorContainerConfiguration();
        }

        public IEnumerable<IDisposable> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container
                .Register<ISession, ITool>(session => new PublisherTool(
                    session,
                    container.ResolveAll<IReportPublisher>(),
                    container.Resolve<IEventAggregator>()));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IReportPublisher>(() => new ReportPublisher(container.ResolveAll<IOutput>()));
        }
    }
}
