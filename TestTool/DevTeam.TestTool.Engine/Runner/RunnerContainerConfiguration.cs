﻿namespace DevTeam.TestTool.Engine.Runner
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

            container = container.Resolve<IContainer>(nameof(RunnerContainerConfiguration));            

            container
                .Register<ISession, ITool>(session => new RunnerTool(
                    container.Resolve<IScheduler>(WellknownScheduler.PrivateSingleThread),
                    session, 
                    container.Resolve<ITestRunner>(),
                    container.Resolve<IEventAggregator>()));

            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register<ITestRunner>(() => new TestRunner(container.Resolve<IReflection>()));

            return container;
        }
    }
}
