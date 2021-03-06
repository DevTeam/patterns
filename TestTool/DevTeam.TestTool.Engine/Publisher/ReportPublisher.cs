﻿namespace DevTeam.TestTool.Engine.Publisher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    internal class ReportPublisher : IReportPublisher
    {
        private readonly IEnumerable<IOutput> _outputs;

        public ReportPublisher(
            IEnumerable<IOutput> outputs)
        {
            if (outputs == null) throw new ArgumentNullException(nameof(outputs));

            _outputs = outputs.ToList();
        }

        public void OnNext(TestReport value)
        {
            Publish(value.Report);
        }

        public void OnNext(SummariseReport value)
        {
            Publish($"Totals: {value.TestTotals}");
            Publish($"Fails: {value.TestFails}");
            Publish($"Success: {value.TestSuccess}");
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        public void Dispose()
        {
        }

        private void Publish(string message)
        {
            foreach (var output in _outputs)
            {
                output.WriteReport(message);
            }
        }

    }
}
