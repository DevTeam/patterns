namespace DevTeam.TestTool.Engine.Reporter
{
    using System;
    using System.Collections.Generic;

    using Patterns.EventAggregator;
    using Patterns.IoC;
    using Contracts;

    public class ReporterContainerConfiguration: IConfiguration
    {
        public static readonly IConfiguration Shared = new ReporterContainerConfiguration();

        private ReporterContainerConfiguration()
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
                .Register<ISession, ITool>(session => new ReporterTool(
                    session,
                    container.ResolveAll<ITestReporter>(),
                    container.Resolve<IEventAggregator>()));

            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<ITestReporter>(() => new TextTestReporter(), "text");
        }
    }
}
