namespace DevTeam.TestTool.Engine.Runner
{
    using System;

    using Patterns.Reactive;
    using Contracts;

    internal class TestsRunner : ITestsRunner
    {                
        public IObservable<TestResult> RunTests(IObservable<Test> testSource, ITestRunner testRunner)
        {
            return testSource.Select(testRunner.Run);
        }
    }
}
