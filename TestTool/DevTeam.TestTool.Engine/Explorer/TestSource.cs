namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Linq;

    using Contracts;

    using Patterns.IoC;
    using Patterns.Reactive;

    using Platform.Reflection;

    using TestTool.Contracts;

    internal class TestSource : ITestSource
    {
        private readonly IReflection _reflection;
        private readonly IObservable<Test> _testSource;

        public TestSource(
            [State] ISession session,
            IReflection reflection,
            [Dependency(Key = WellknownProperty.Assembly)] IProperty assemblyProperty)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (reflection == null) throw new ArgumentNullException(nameof(reflection));
            if (assemblyProperty == null) throw new ArgumentNullException(nameof(assemblyProperty));

            _reflection = reflection;

            _testSource = (
                from assemblyFileName in
                session.Properties.Where(p => Equals(p.Property, assemblyProperty)).Select(p => p.Value)
                let assembly = _reflection.LoadAssembly(assemblyFileName)
                let testAssembly = new TestAssembly(assemblyFileName)
                from type in assembly.DefinedTypes
                let testFixtureAttribute = type.GetCustomAttributes<TestFixtureAttribute>().SingleOrDefault()
                where testFixtureAttribute != null
                let testFixture = new TestFixture(testAssembly, type.FullName)
                from method in type.Methods
                let testAttributes = method.GetCustomAttributes<TestAttribute>()
                where testAttributes != null && testAttributes.Any()
                let testMethod = new TestMethod(testFixture, method.Name)
                select new Test(testMethod)).ToObservable();
        }

        public IDisposable Subscribe(IObserver<Test> observer)
        {
            return _testSource.Subscribe(observer);
        }
    }
}
