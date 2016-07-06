namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Dispose;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    using Platform.Reflection;

    public class ExplorerContainerConfiguration: IConfiguration
    {
        public static readonly IConfiguration Shared = new ExplorerContainerConfiguration();

        private ExplorerContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return ReactiveContainerConfiguration.Shared;
            yield return EventAggregatorContainerConfiguration.Shared;
            yield return ReflectionContainerConfiguration.Shared;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return
                container
                    .Register<ISession, ITool>(session => new ExplorerTool(
                        container.Resolve<IScheduler>(WellknownScheduler.PrivateSingleThread),
                        session, 
                        container.Resolve<IEventAggregator>(), 
                        container.ResolveAll<ISession, ITestsSource>(name => session)));

            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<ISession, ITestsSource>(session => new AssemblyTestsSource(session , container.Resolve<IReflection>()));
        }
    }
}
