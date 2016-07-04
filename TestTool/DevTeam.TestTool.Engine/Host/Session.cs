namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Contracts;

    using Patterns.EventAggregator;

    using Patterns.Dispose;
    using Patterns.IoC;

    internal class Session : ISession
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Session(
            IResolver<ISession, ITool> toolResolver, 
            IEventAggregator eventAggregator,
            IReportPublisher reportPublisher,
            IEnumerable<IPropertyValue> properties)
        {
            if (toolResolver == null) throw new ArgumentNullException(nameof(toolResolver));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (reportPublisher == null) throw new ArgumentNullException(nameof(reportPublisher));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            Properties = new ReadOnlyCollection<IPropertyValue>(new List<IPropertyValue>(properties));
            _disposable.Add(eventAggregator.RegisterConsumer(reportPublisher));
            foreach (var tool in GetToolNames().Select(toolName => toolResolver.Resolve(this, toolName)).OrderByDescending(tool => tool.ToolType))
            {
                _disposable.Add(tool.Activate());
            }            
        }

        public IEnumerable<IPropertyValue> Properties { get; }

        public void Dispose()
        {            
            _disposable.Dispose();
        }

        private IEnumerable<string> GetToolNames()
        {
            return Properties.Where(p => Equals(p.Property, ToolProperty.Shared)).Select(i => i.Value);            
        }
    }
}
