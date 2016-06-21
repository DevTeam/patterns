namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Contracts;

    using Patterns.Dispose;
    using Patterns.IoC;

    internal class Session : ISession
    {
        private readonly IContainer _container;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Session(
            IContainer container, 
            IEnumerable<PropertyValue> properties)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            
            _container = container;
            Properties = new ReadOnlyCollection<PropertyValue>(new List<PropertyValue>(properties));

            foreach (var toolName in GetToolNames())
            {
                _disposable.Add(CreateTool(toolName).Run());
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

            if (string.IsNullOrEmpty(toolName))
            {
                var toolProperty = Properties.SingleOrDefault(p => Equals(p.Property, ToolProperty.Shared));
                toolName = toolProperty.Value;
            }

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
