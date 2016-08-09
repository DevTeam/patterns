namespace DevTeam.TestTool.Engine.Publisher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Patterns.Dispose;
    using Patterns.EventAggregator;
    using Contracts;

    using Patterns.IoC;

    internal class PublisherTool : ITool
    {
        private readonly ISession _session;
        private readonly IEnumerable<IReportPublisher> _reportPublishers;
        private readonly IEventAggregator _eventAggregator;

        public PublisherTool(
            [State] ISession session,
            IEnumerable<IReportPublisher> reportPublishers,
            IEventAggregator eventAggregator)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));

            _session = session;
            _reportPublishers = reportPublishers.ToList();
            _eventAggregator = eventAggregator;
        }

        public ToolType ToolType => ToolType.Publisher;

        public IDisposable Activate()
        {
            return 
                _reportPublishers
                .OfType<IObserver<TestReport>>()
                .Select(publisher => _eventAggregator.RegisterConsumer(publisher))
                .Concat(
                    _reportPublishers
                    .OfType<IObserver<SummariseReport>>()
                    .Select(publisher => _eventAggregator.RegisterConsumer(publisher)))
                .ToCompositeDisposable();
        }
    }
}
