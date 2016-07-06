namespace DevTeam.TestTool.Engine.Runner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Patterns.EventAggregator;
    using Contracts;

    using Patterns.Dispose;

    internal class RunnerTool: ITool
    {
        private readonly ISession _session;
        private readonly IEnumerable<ITestRunner> _testRunners;
        private readonly IEventAggregator _eventAggregator;        

        public RunnerTool(
            ISession session,
            IEnumerable<ITestRunner> testRunners,
            IEventAggregator eventAggregator)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));

            _session = session;
            _testRunners = testRunners;
            _eventAggregator = eventAggregator;            
        }

        public ToolType ToolType => ToolType.Runner;

        public IDisposable Activate()
        {
            return (
                from runner in _testRunners
                select _eventAggregator.RegisterConsumer(runner))
            .Concat(
                from runner in _testRunners
                select _eventAggregator.RegisterProvider(runner))
            .ToCompositeDisposable();            
        }
       
    }
}