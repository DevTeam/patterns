namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Linq;

    using Abstractions;

    using Contracts;

    using Patterns.Reactive;

    using Host;

    using TestTool.Contracts;

    internal class AssemblyTestsSource : ITestsSource
    {
        private readonly IReflection _reflection;
        private readonly IObservable<Test> _testSource;

        public AssemblyTestsSource(ISession session, IReflection reflection)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (reflection == null) throw new ArgumentNullException(nameof(reflection));

            _reflection = reflection;

            _testSource = (
                from assemblyFileName in
                session.Properties.Where(p => Equals(p.Property, AssemblyProperty.Shared)).Select(p => p.Value)
                let assembly = _reflection.LoadAssembly(assemblyFileName)
                let testAssembly = new TestAssembly(assemblyFileName)
                from type in assembly.DefinedTypes
                let testFixtureAttribute = type.GetCustomAttributes<TestFixtureAttribute>().SingleOrDefault()
                where testFixtureAttribute != null
                let testFixture = new TestFixture(testAssembly, type.FullName)
                from method in type.Methods
                let testAttribute = method.GetCustomAttributes<TestAttribute>()
                where testAttribute != null
                let testMethod = new TestMethod(testFixture, method.Name)
                select new Test(testMethod)).ToObservable();
        }

        public IDisposable Subscribe(IObserver<Test> observer)
        {
            return _testSource.Subscribe(observer);
        }
    }
}
