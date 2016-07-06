namespace DevTeam.TestTool.Engine.Runner
{
    using System;

    using Abstractions;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Dispose;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    public class RunnerContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(container
                .Register<ISession, ITool>(session => new RunnerTool(
                    session, 
                    container.ResolveAll<ITestRunner>(),
                    container.Resolve<IEventAggregator>())));

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<ITestRunner>(() => new TestRunner(container.Resolve<IReflection>())));

            return disposable;
        }
    }
}
