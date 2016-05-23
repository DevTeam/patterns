namespace DevTeam.TestTool.Engine.Explorer
{
    using System;

    using Patterns.IoC;
    using Contracts;

    using Patterns.EventAggregator;

    public class ContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var testSource = new Lazy<ITestsSource>(() => new TestsSource(container.Resolve<IAssemblyLoader>()));

            container
                .Register<ISession, ITool>(session => new ExplorerTool(session, container.Resolve<IEventAggregator>(), container.Resolve<ITestsSource>()))
                .Register(() => testSource.Value);

            return container;
        }
    }
}
