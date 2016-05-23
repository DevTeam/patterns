namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public interface ITestsSource
    {
        IObservable<Test> Create(ISession session);
    }
}