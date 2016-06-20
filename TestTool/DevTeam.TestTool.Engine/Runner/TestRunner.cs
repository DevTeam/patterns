namespace DevTeam.TestTool.Engine.Runner
{
    using System;

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
                var testFixtureType = _reflection.LoadType(testAssembly, test.Method.Fixture.Name);
                var methodInfo = _reflection.LoadMethod(testFixtureType, test.Method.Name);
                var testInstance = _reflection.CreateInstance(testFixtureType);
                try
                {
                    _results.OnNext(new TestProgress(TestState.Starting));
                    var result = methodInfo.Invoke(testInstance, null);
                    _results.OnNext(new TestProgress(TestState.Finished, new TestResult(test, result)));
                }
                catch (Exception exception)
                {
                    _results.OnNext(new TestProgress(TestState.Finished, new TestResult(test, exception)));                    
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
