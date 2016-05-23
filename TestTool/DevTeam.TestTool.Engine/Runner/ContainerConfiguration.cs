namespace DevTeam.TestTool.Engine.Runner
{
    using System;

    using Patterns.IoC;
    using Contracts;

    public class ContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            var testEngine = new Lazy<ITestsRunner>(() => new TestsRunner());
            var testRunner = new Lazy<ITestRunner>(() => new TestRunner(
                container.Resolve<IAssemblyLoader>(),
                container.Resolve<ITypeLoader>(),
                container.Resolve<IMethodInfoLoader>(),
                container.Resolve<IInstanceFactory>()));
            var assemblyLoader = new Lazy<IAssemblyLoader>(() => new AssemblyLoader());
            var typeLoader = new Lazy<ITypeLoader>(() => new TypeLoader());
            var methodInfoLoader = new Lazy<IMethodInfoLoader>(() => new MethodInfoLoader());
            var instanceFactory = new Lazy<IInstanceFactory>(() => new InstanceFactory());

            container
                .Register(() => testEngine.Value)
                .Register(() => testRunner.Value)
                .Register(() => assemblyLoader.Value)
                .Register(() => typeLoader.Value)
                .Register(() => methodInfoLoader.Value)
                .Register(() => instanceFactory.Value);

            return container;
        }
    }
}
