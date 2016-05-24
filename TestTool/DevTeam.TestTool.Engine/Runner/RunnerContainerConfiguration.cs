namespace DevTeam.TestTool.Engine.Runner
{
    using System;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    public class RunnerContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container = container.Resolve<IContainer>(typeof(RunnerContainerConfiguration).Name);

            var testRunner = new Lazy<ITestRunner>(() => new TestRunner(container.Resolve<IReflection>()));
           
            container
                .Register<ISession, ITool>(session => new RunnerTool(
                    container.Resolve<IScheduler>(WellknownSchedulers.PrivateSingleThread),
                    session, 
                    container.Resolve<ITestRunner>(),
                    container.Resolve<IEventAggregator>()))
                .Register(() => testRunner.Value);

            return container;
        }
    }
}
