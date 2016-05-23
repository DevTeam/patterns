namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public interface ITestsRunner
    {
        IObservable<TestResult> RunTests(IObservable<Test> testSource, ITestRunner testRunner);
    }
}