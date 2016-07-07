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
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new EventAggregatorContainerConfiguration();
            yield return new ReflectionContainerConfiguration();
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container
                .Register<ISession, ITool>(session => new RunnerTool(
                    session, 
                    container.ResolveAll<ITestRunner>(),
                    container.Resolve<IEventAggregator>()));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<ITestRunner>(() => new TestRunner(container.Resolve<IReflection>()));
        }
    }
}
