namespace DevTeam.TestTool.Engine.Runner
{
    using System;

    using Contracts;

    internal class TestRunner : ITestRunner
    {
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly ITypeLoader _typeLoader;
        private readonly IMethodInfoLoader _methodInfoLoader;
        private readonly IInstanceFactory _instanceFactory;

        public TestRunner(
            IAssemblyLoader assemblyLoader,
            ITypeLoader typeLoader,
            IMethodInfoLoader methodInfoLoader,
            IInstanceFactory instanceFactory)
        {
            if (assemblyLoader == null) throw new ArgumentNullException(nameof(assemblyLoader));
            if (typeLoader == null) throw new ArgumentNullException(nameof(typeLoader));
            if (methodInfoLoader == null) throw new ArgumentNullException(nameof(methodInfoLoader));

            _assemblyLoader = assemblyLoader;
            _typeLoader = typeLoader;
            _methodInfoLoader = methodInfoLoader;
            _instanceFactory = instanceFactory;
        }

        public TestResult Run(Test test)
        {
            var testAssembly = _assemblyLoader.Load(test.Fixture.Assembly);
            var testFixtureType = _typeLoader.Load(testAssembly, test.Fixture);
            var methodInfo = _methodInfoLoader.Load(testFixtureType, test.Method);
            var testInstance = _instanceFactory.Create(testFixtureType);
            var result = methodInfo.Invoke(testInstance, null);
            return new TestResult(test, result);
        }
    }
}
