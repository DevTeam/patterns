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
        public static readonly IConfiguration Shared = new PublisherContainerConfiguration();

        private PublisherContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return EventAggregatorContainerConfiguration.Shared;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container
                .Register<ISession, ITool>(session => new PublisherTool(
                    session,
                    container.ResolveAll<IReportPublisher>(),
                    container.Resolve<IEventAggregator>()));

            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IReportPublisher>(() => new ReportPublisher(container.ResolveAll<IOutput>()));
        }
    }
}
