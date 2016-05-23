namespace DevTeam.TestTool.Engine.Explorer
{
    using System;

    using Patterns.IoC;
    using Contracts;

    public class ContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            var testSource = new Lazy<ITestsSource>(() => new TestsSource());
            container
                .Register<ISession, ITool>(session => new ExplorerTool(session))
                .Register(() => testSource.Value);

            return container;
        }
    }
}
