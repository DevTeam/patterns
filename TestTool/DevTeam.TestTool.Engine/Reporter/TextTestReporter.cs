namespace DevTeam.TestTool.Engine.Reporter
{
    using System;

    using Contracts;

    internal class TextTestReporter: ITestReporter
    {
        public IDisposable Subscribe(IObserver<TestReport> observer)
        {
            throw new NotImplementedException();
        }

        public void OnNext(TestProgress value)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}