namespace DevTeam.TestTool.Engine.Reporter
{
    using System;

    using Contracts;

    using Patterns.Dispose;
    using Patterns.IoC;
    using Patterns.Reactive;

    internal class SummariseTestReporter : ITestReporter
    {
        private readonly ISubject<SummariseReport> _summariseReportSubject;
        private int _testTotals;
        private int _testFails;
        private int _testSuccess;

        public SummariseTestReporter(
            [Dependency(Key = WellknownSubject.Simple)] ISubject<SummariseReport> subject)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));

            _summariseReportSubject = subject;
        }

        public IDisposable Subscribe(IObserver<TestReport> observer)
        {
            return Disposable.Empty();
        }

        public IDisposable Subscribe(IObserver<SummariseReport> observer)
        {
            return _summariseReportSubject.Subscribe(observer);
        }

        public void OnNext(TestProgress value)
        {
            switch (value.TestState)
            {
                case TestState.Starting:
                    _testTotals++;
                    break;

                case TestState.Finished:
                    if (value.Result.Exception != null)
                    {
                        _testFails++;
                    }
                    else
                    {
                        _testSuccess++;
                    }
                    break;
            }
        }

        public void OnError(Exception error)
        {
            _summariseReportSubject.OnError(error);
        }

        public void OnCompleted()
        {
            _summariseReportSubject.OnNext(new SummariseReport(_testTotals, _testFails, _testSuccess));
            _summariseReportSubject.OnCompleted();
        }
    }
}