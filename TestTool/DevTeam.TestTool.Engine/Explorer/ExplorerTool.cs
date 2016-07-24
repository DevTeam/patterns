namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    using Patterns.Dispose;

    using Patterns.Reactive;

    using Patterns.EventAggregator;
    using Patterns.IoC;

    internal class ExplorerTool: ITool
    {
        private readonly IScheduler _scheduler;
        private readonly ISession _session;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEnumerable<ITestSource> _testsSources;
        private readonly IResolver<ISubject<Test>> _testSubjectResolver;

        public ExplorerTool(
            [Dependency(Key = WellknownScheduler.PrivateSingleThread)] IScheduler scheduler,
            [State] ISession session, 
            IEventAggregator eventAggregator,
            IResolver<ISession, IEnumerable<ITestSource>> testsSources,
            IResolver<ISubject<Test>> testSubjectResolver)
        {
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (testsSources == null) throw new ArgumentNullException(nameof(testsSources));
            if (testSubjectResolver == null) throw new ArgumentNullException(nameof(testSubjectResolver));

            _scheduler = scheduler;
            _session = session;
            _eventAggregator = eventAggregator;
            _testsSources = testsSources.Resolve(session);
            _testSubjectResolver = testSubjectResolver;
        }

        public ToolType ToolType => ToolType.Explorer;

        public IDisposable Activate()
        {
            var testSource = _testsSources.Aggregate(Observable.Empty<Test>(), (currentSource, nextSource) => currentSource.Concat(nextSource)).SubscribeOn(_scheduler);
            var testSubject = _testSubjectResolver.Resolve(WellknownSubject.Simple);
            return new CompositeDisposable(
                _eventAggregator.RegisterProvider(testSubject),
                testSource.Subscribe(testSubject),
                Disposable.Create(() => testSubject.WaitForCompletion()));
        }
    }
}