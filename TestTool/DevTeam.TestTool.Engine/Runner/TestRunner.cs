namespace DevTeam.TestTool.Engine.Runner
{
    using System;
    using System.Linq;

    using Abstractions;
    using Abstractions.Reflection;

    using Contracts;

    using Patterns.Reactive;

    internal class TestRunner : ITestRunner
    {
        private readonly Subject<TestProgress> _results = new Subject<TestProgress>();
        private readonly IReflection _reflection;

        public TestRunner(IReflection reflection)
        {
            if (reflection == null) throw new ArgumentNullException(nameof(reflection));

            _reflection = reflection;
        }
        
        public void OnNext(Test test)
        {
            if (test == null) throw new ArgumentNullException(nameof(test));

            try
            {
                var testAssembly = _reflection.LoadAssembly(test.Method.Fixture.Assembly.Name);
                var testFixtureType = testAssembly.GetType(test.Method.Fixture.Name);
                var methodInfo = testFixtureType.Methods.SingleOrDefault(method => method.Name == test.Method.Name);
                var testInstance = testFixtureType.CreateInstance();
                try
                {
                    _results.OnNext(new TestProgress(test, TestState.Starting));
                    var result = methodInfo.Invoke(testInstance);
                    _results.OnNext(new TestProgress(test, TestState.Finished, new TestResult(result)));
                }
                catch (Exception exception)
                {
                    _results.OnNext(new TestProgress(test, TestState.Finished, new TestResult(exception)));                    
                }                
            }
            catch (Exception ex)
            {
                _results.OnError(ex);
            }
        }

        public void OnError(Exception error)
        {         
            _results.OnError(error);
        }

        public void OnCompleted()
        {
            _results.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<TestProgress> observer)
        {
            return _results.Subscribe(observer);
        }
    }
}
