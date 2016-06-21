namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Contracts;

    using DevTeam.Patterns.EventAggregator;

    using Patterns.Dispose;
    using Patterns.IoC;

    internal class Session : ISession
    {
        private readonly IContainer _container;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Session(
            IContainer container, 
            IEventAggregator eventAggregator,
            IReportPublisher reportPublisher,
            IEnumerable<PropertyValue> properties)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (reportPublisher == null) throw new ArgumentNullException(nameof(reportPublisher));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            
            _container = container;
            Properties = new ReadOnlyCollection<PropertyValue>(new List<PropertyValue>(properties));

            _disposable.Add(eventAggregator.RegisterConsumer(reportPublisher));
            foreach (var tool in GetToolNames().Select(CreateTool).OrderByDescending(tool => tool.ToolType))
            {
                _disposable.Add(tool.Activate());
            }            
        }

        public IEnumerable<PropertyValue> Properties { get; }

        public void Dispose()
        {            
            _disposable.Dispose();
        }

        private ITool CreateTool(string toolName)
        {
            if (toolName == null) throw new ArgumentNullException(nameof(toolName));
            var tooContainerConfiguration = _container.Resolve<IConfiguration>(toolName);
            var tooContainer = _container.Resolve<IContainer>(toolName);
            tooContainerConfiguration.Apply(tooContainer);
            return tooContainer.Resolve<ISession, ITool>(this);
        }

        private IEnumerable<string> GetToolNames()
        {
            return Properties.Where(p => Equals(p.Property, ToolProperty.Shared)).Select(i => i.Value);            
        }
    }
}
