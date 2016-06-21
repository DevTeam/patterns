using System;
using DevTeam.TestTool.Engine.Contracts;

namespace DevTeam.TestTool.Engine.Host
{
    internal interface IReportPublisher: IDisposable, IObserver<TestReport>
    {        
    }
}