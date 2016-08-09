namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public interface IReportPublisher : IDisposable, IObserver<TestReport>, IObserver<SummariseReport>
    {
    }
}