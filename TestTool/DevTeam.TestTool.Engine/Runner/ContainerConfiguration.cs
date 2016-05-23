namespace DevTeam.TestTool.Engine.Runner
{
    using System;

    using Patterns.IoC;
    using Contracts;

    public class ContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var testEngine = new Lazy<ITestsRunner>(() => new TestsRunner());
            var testRunner = new Lazy<ITestRunner>(() => new TestRunner(
                container.Resolve<IAssemblyLoader>(),
                container.Resolve<ITypeLoader>(),
                container.Resolve<IMethodInfoLoader>(),
                container.Resolve<IInstanceFactory>()));
           
            container
                .Register(() => testEngine.Value)
                .Register(() => testRunner.Value);

            return container;
        }
    }
}
