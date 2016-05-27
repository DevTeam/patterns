namespace DevTeam.TestTool.Engine.Explorer
{
    using System;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    public class ExplorerContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container = container.Resolve<IContainer>(nameof(ExplorerContainerConfiguration));            

            container
                .Register<ISession, ITool>(session => new ExplorerTool(
                    container.Resolve<IScheduler>(WellknownScheduler.PrivateSingleThread),
                    session, 
                    container.Resolve<IEventAggregator>(), 
                    container.Resolve<ITestsSource>()));

            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register<ITestsSource>(() => new TestsSource(container.Resolve<IReflection>()));

            return container;
        }
    }
}
