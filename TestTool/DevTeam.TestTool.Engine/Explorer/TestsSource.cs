namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Linq;

    using Patterns.Reactive;
    using Contracts;

    using TestTool.Contracts;

    internal class TestsSource : ITestsSource
    {
        private readonly IReflection _reflection;

        public TestsSource(IReflection reflection)
        {
            if (reflection == null) throw new ArgumentNullException(nameof(reflection));

            _reflection = reflection;
        }

        public IObservable<Test> Create(ISession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return (
                from assemblyFileName in
                session.Properties.Where(p => Equals(p.Property, AssemblyProperty.Shared)).Select(p => p.Value)
                let assembly = _reflection.LoadAssembly(assemblyFileName)
                let testAssembly = new TestAssembly(assemblyFileName)
                from type in _reflection.GetTypes(assembly)
                let testFixtureAttribute = _reflection.GetCustomAttribute<TestFixtureAttribute>(type).SingleOrDefault()
                where testFixtureAttribute != null
                let testFixture = new TestFixture(testAssembly, type.FullName)
                from method in _reflection.GetMethods(type)
                let testAttribute = _reflection.GetCustomAttribute<TestAttribute>(method).SingleOrDefault()
                where testAttribute != null
                let testMethod = new TestMethod(testFixture, method.Name)
                select new Test(testMethod)
                ).ToObservable();
        }
    }
}
