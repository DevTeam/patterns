namespace DevTeam.TestTool.Engine.Publisher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Patterns.Dispose;
    using Patterns.EventAggregator;
    using Contracts;

    using Host;

    internal class PublisherTool: ITool
    {
        private readonly ISession _session;
        private readonly IEnumerable<IReportPublisher> _reportPublishers;
        private readonly IEventAggregator _eventAggregator;

        public PublisherTool(
            ISession session,
            IEnumerable<IReportPublisher> reportPublishers,
            IEventAggregator eventAggregator)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));

            _session = session;
            _reportPublishers = reportPublishers;
            _eventAggregator = eventAggregator;
        }

        public ToolType ToolType => ToolType.Publisher;

        public IDisposable Activate()
        {
            return (
                from publisher in _reportPublishers
                select _eventAggregator.RegisterConsumer(publisher))
            .ToCompositeDisposable();            
        }
    }
}
