namespace DevTeam.TestTool.Engine.Contracts
{
    using System;
    public interface ITestRunner: IObserver<Test>, IObservable<TestResult>
    {        
    }
}