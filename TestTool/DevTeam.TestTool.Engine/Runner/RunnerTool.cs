namespace DevTeam.TestTool.Engine.Runner
{
    using System;

    using Patterns.EventAggregator;
    using Contracts;

    using Patterns.Dispose;

    using Patterns.Reactive;

    internal class RunnerTool: ITool
    {
        private readonly IScheduler _scheduler;
        private readonly ISession _session;
        private readonly ITestRunner _testRunner;
        private readonly IEventAggregator _eventAggregator;        

        public RunnerTool(
            IScheduler scheduler,
            ISession session,
            ITestRunner testRunner,
            IEventAggregator eventAggregator)
        {
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));

            _scheduler = scheduler;
            _session = session;
            _testRunner = testRunner;
            _eventAggregator = eventAggregator;            
        }

        public IDisposable Run()
        {
            return _eventAggregator.RegisterConsumer(((IObserver<Test>)_testRunner).ObserveOn(_scheduler));
        }

        public void OnNext(Test value)
        {         
        }

        public void OnError(Exception error)
        {            
        }

        public void OnCompleted()
        {         
        }
    }
}