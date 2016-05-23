namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Patterns.Reactive;
    using Contracts;

    using TestTool.Contracts;

    internal class TestsSource : ITestsSource
    {
        private readonly IAssemblyLoader _assemblyLoader;

        public TestsSource(
            IAssemblyLoader assemblyLoader)
        {
            if (assemblyLoader == null) throw new ArgumentNullException(nameof(assemblyLoader));

            _assemblyLoader = assemblyLoader;
        }

        public IObservable<Test> Create(ISession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return (
                from assemblyName in
                session.Properties.Where(p => Equals(p.Property, AssemblyProperty.Shared)).Select(p => p.Value)
                let assembly = _assemblyLoader.Load(assemblyName)
                let testAssembly = new TestAssembly(assembly.FullName)
                from type in assembly.DefinedTypes
                let testFixtureAttribute = type.GetCustomAttribute<TestFixtureAttribute>()
                where testFixtureAttribute != null
                let testFixture = new TestFixture(testAssembly, type.FullName)
                from method in type.DeclaredMethods
                let testAttribute = method.GetCustomAttribute<TestAttribute>()
                where testAttribute != null
                let testMethod = new TestMethod(testFixture, method.Name)
                select new Test(testMethod)
                ).ToObservable();
        }
    }
}
