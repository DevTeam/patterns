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

            container = container.Resolve<IContainer>(typeof(ExplorerContainerConfiguration).Name);
            var testSource = new Lazy<ITestsSource>(() => new TestsSource(container.Resolve<IReflection>()));

            container
                .Register<ISession, ITool>(session => new ExplorerTool(
                    container.Resolve<IScheduler>(WellknownSchedulers.PrivateSingleThread),
                    session, 
                    container.Resolve<IEventAggregator>(), 
                    container.Resolve<ITestsSource>()))
                .Register(() => testSource.Value);

            return container;
        }
    }
}
