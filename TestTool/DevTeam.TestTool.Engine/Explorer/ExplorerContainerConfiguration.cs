namespace DevTeam.TestTool.Engine.Explorer
{
    using System;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Dispose;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    public class ExplorerContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(
                container
                    .Register<ISession, ITool>(session => new ExplorerTool(
                        container.Resolve<IScheduler>(WellknownScheduler.PrivateSingleThread),
                        session, 
                        container.Resolve<IEventAggregator>(), 
                        container.Resolve<ITestsSource>())));

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<ITestsSource>(() => new TestsSource(container.Resolve<IReflection>())));

            return disposable;
        }
    }
}
