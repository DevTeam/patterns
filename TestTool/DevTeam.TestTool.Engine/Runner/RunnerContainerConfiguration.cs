namespace DevTeam.TestTool.Engine.Runner
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    using Patterns.EventAggregator;

    using Platform.Reflection;

    public class RunnerContainerConfiguration: IConfiguration
    {
        public static readonly IConfiguration Shared = new RunnerContainerConfiguration();

        private RunnerContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return EventAggregatorContainerConfiguration.Shared;
            yield return ReflectionContainerConfiguration.Shared;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container
                .Register<ISession, ITool>(session => new RunnerTool(
                    session, 
                    container.ResolveAll<ITestRunner>(),
                    container.Resolve<IEventAggregator>()));

            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<ITestRunner>(() => new TestRunner(container.Resolve<IReflection>()));
        }
    }
}
