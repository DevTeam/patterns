namespace DevTeam.TestTool.Engine.Reporter
{
    using System;

    using Contracts;

    using Patterns.Dispose;
    using Patterns.IoC;
    using Patterns.Reactive;

    internal class TextTestReporter : ITestReporter
    {
        private readonly ISubject<TestReport> _testReportSubject;

        public TextTestReporter(
            [Dependency(Key = WellknownSubject.Simple)] ISubject<TestReport> subject)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));

            _testReportSubject = subject;
        }

        public IDisposable Subscribe(IObserver<TestReport> observer)
        {
            return _testReportSubject.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<SummariseReport> observer)
        {
            return Disposable.Empty();
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