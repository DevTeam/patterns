namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    internal class ExplorerTool: ITool
    {
        private readonly IScheduler _scheduler;

        private readonly ISession _session;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEnumerable<ITestsSource> _testsSources;

        public ExplorerTool(
            IScheduler scheduler,
            ISession session, 
            IEventAggregator eventAggregator,
            IEnumerable<ITestsSource> testsSources)
        {
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (testsSources == null) throw new ArgumentNullException(nameof(testsSources));

            _scheduler = scheduler;
            _session = session;
            _eventAggregator = eventAggregator;
            _testsSources = testsSources;
        }

        public ToolType ToolType => ToolType.Explorer;

        public IDisposable Activate()
        {
            var testSource = _testsSources.Aggregate(Observable.Empty<Test>(), (currentSource, nextSource) => currentSource.Concat(nextSource.Create(_session)));
            return _eventAggregator.RegisterProvider(testSource);
        }
    }
}