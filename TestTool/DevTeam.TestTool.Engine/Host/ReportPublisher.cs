namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Contracts;

    internal class ReportPublisher : IReportPublisher
    {
        private readonly IEnumerable<IOutput> _outputs;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        public ReportPublisher(
            IEnumerable<IOutput> outputs)
        {
            if (outputs == null) throw new ArgumentNullException(nameof(outputs));

            _outputs = outputs;
        }

        public void OnNext(TestReport value)
        {
            foreach (var output in _outputs)
            {
                output.Write(value.Report);
            }
        }

        public void OnError(Exception error)
        {
            _completedEvent.Set();
        }

        public void OnCompleted()
        {
            _completedEvent.Set();
        }

        public void Dispose()
        {
            _completedEvent.WaitOne();
        }
    }
}
