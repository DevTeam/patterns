namespace DevTeam.TestTool.Engine.Reporter
{
    using System;

    using Contracts;

    using DevTeam.Patterns.Reactive;

    internal class TextTestReporter: ITestReporter
    {
        private readonly Subject<TestReport> _testReportSubject = new Subject<TestReport>();

        public IDisposable Subscribe(IObserver<TestReport> observer)
        {
            return _testReportSubject.Subscribe(observer);
        }

        public void OnNext(TestProgress value)
        {            
        }

        public void OnError(Exception error)
        {
            _testReportSubject.OnError(error);
        }

        public void OnCompleted()
        {
            _testReportSubject.OnCompleted();         
        }
    }
}