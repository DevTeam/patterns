namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    using Platform.Reflection;

    public class ExplorerContainerConfiguration: IConfiguration
    {
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new ReactiveContainerConfiguration();
            yield return new EventAggregatorContainerConfiguration();
            yield return new ReflectionContainerConfiguration();
        }

        public IEnumerable<IDisposable> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return
                container
                    .Register<ISession, ITool>(session => new ExplorerTool(
                        container.Resolve<IScheduler>(WellknownScheduler.PrivateSingleThread),
                        session, 
                        container.Resolve<IEventAggregator>(), 
                        container.ResolveAll<ISession, ITestsSource>(name => session)));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<ISession, ITestsSource>(session => new AssemblyTestsSource(session , container.Resolve<IReflection>()));
        }
    }
}
