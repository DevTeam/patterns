namespace DevTeam.TestTool.Engine.Explorer
{
    using System;

    using Contracts;

    using Patterns.Reactive;

    using Patterns.EventAggregator;

    internal class ExplorerTool : ITool
    {
        private readonly IScheduler _scheduler;

        private readonly ISession _session;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITestsSource _testsSource;

        public ExplorerTool(
            IScheduler scheduler,
            ISession session, 
            IEventAggregator eventAggregator,
            ITestsSource testsSource)
        {
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (testsSource == null) throw new ArgumentNullException(nameof(testsSource));

            _scheduler = scheduler;
            _session = session;
            _eventAggregator = eventAggregator;
            _testsSource = testsSource;
        }

        public IDisposable Run()
        {
            var testSource = _testsSource.Create(_session).SubscribeOn(_scheduler);
            return _eventAggregator.RegisterProvider(testSource);            
        }
    }
}