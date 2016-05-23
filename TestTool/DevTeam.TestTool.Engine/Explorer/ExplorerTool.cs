namespace DevTeam.TestTool.Engine.Explorer
{
    using System;

    using Contracts;

    using Patterns.EventAggregator;

    internal class ExplorerTool : ITool
    {
        private readonly ISession _session;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITestsSource _testsSource;

        public ExplorerTool(
            ISession session, 
            IEventAggregator eventAggregator,
            ITestsSource testsSource)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (testsSource == null) throw new ArgumentNullException(nameof(testsSource));

            _session = session;
            _eventAggregator = eventAggregator;
            _testsSource = testsSource;
        }

        public void Run()
        {
            var testSource = _testsSource.Create(_session);
            _eventAggregator.RegisterProvider(testSource);            
        }
    }
}