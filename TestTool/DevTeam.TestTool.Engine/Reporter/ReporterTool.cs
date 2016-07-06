namespace DevTeam.TestTool.Engine.Reporter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    using Patterns.EventAggregator;

    using Patterns.Dispose;

    internal class ReporterTool: ITool
    {
        private readonly ISession _session;
        private readonly IEnumerable<ITestReporter> _testReporters;
        private readonly IEventAggregator _eventAggregator;

        public ReporterTool(
            ISession session,
            IEnumerable<ITestReporter> testReporters,
            IEventAggregator eventAggregator)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (testReporters == null) throw new ArgumentNullException(nameof(testReporters));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));

            _session = session;
            _testReporters = testReporters;
            _eventAggregator = eventAggregator;
        }

        public ToolType ToolType => ToolType.Reporter;

        public IDisposable Activate()
        {
            return (
                from reporter in _testReporters
                select _eventAggregator.RegisterConsumer(reporter)).
            Concat(
                from reporter in _testReporters
                select _eventAggregator.RegisterProvider(reporter))
            .ToCompositeDisposable();
        }
    }
}
