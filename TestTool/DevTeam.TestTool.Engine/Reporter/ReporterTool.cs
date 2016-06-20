﻿namespace DevTeam.TestTool.Engine.Reporter
{
    using System;
    using System.Collections.Generic;

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

        public IDisposable Run()
        {
            var disposable = new CompositeDisposable();
            foreach (var testReporter in _testReporters)
            {
                disposable.Add(_eventAggregator.RegisterConsumer(testReporter));                
            }
            
            return disposable;
        }
    }
}
