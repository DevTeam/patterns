namespace DevTeam.TestTool.Engine.Runner
{
    using System;

    using Patterns.EventAggregator;
    using Contracts;

    using Patterns.Dispose;

    internal class RunnerTool: ITool
    {
        private readonly ISession _session;
        private readonly ITestRunner _testRunner;
        private readonly IEventAggregator _eventAggregator;        

        public RunnerTool(
            ISession session,
            ITestRunner testRunner,
            IEventAggregator eventAggregator)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));

            _session = session;
            _testRunner = testRunner;
            _eventAggregator = eventAggregator;            
        }

        public ToolType ToolType => ToolType.Runner;

        public IDisposable Activate()
        {
            return new CompositeDisposable(
                    _eventAggregator.RegisterProvider(_testRunner),
                    _eventAggregator.RegisterConsumer(_testRunner)                    
                );            
        }
       
    }
}