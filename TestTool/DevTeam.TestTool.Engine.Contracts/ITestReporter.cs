namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public interface ITestReporter: IObserver<TestProgress>, IObservable<TestReport>, IObservable<SummariseReport>
    {
    }
}