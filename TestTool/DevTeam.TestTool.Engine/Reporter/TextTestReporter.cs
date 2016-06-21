namespace DevTeam.TestTool.Engine.Reporter
{
    using System;

    using Contracts;

    using Patterns.Reactive;

    internal class TextTestReporter: ITestReporter
    {
        private readonly Subject<TestReport> _testReportSubject = new Subject<TestReport>();

        public IDisposable Subscribe(IObserver<TestReport> observer)
        {
            return _testReportSubject.Subscribe(observer);
        }

        public void OnNext(TestProgress value)
        {
            switch (value.TestState)
            {
                case TestState.Starting:
                    _testReportSubject.OnNext(new TestReport(value.Test, $"{value.Test} - starting"));
                    break;

                case TestState.Finished:
                    _testReportSubject.OnNext(new TestReport(value.Test, $"{value.Test} - finished"));
                    break;
            }            
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