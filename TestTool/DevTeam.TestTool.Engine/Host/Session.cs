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
        private readonly IDisposable _disposable;

        public Session(
            IResolver<ISession, ITool> toolResolver, 
            IEnumerable<IPropertyValue> properties)
        {
            if (toolResolver == null) throw new ArgumentNullException(nameof(toolResolver));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            Properties = new ReadOnlyCollection<IPropertyValue>(new List<IPropertyValue>(properties));
            _disposable = (
                from toolName in GetToolNames()
                let tool = toolResolver.Resolve(this, toolName)
                orderby tool.ToolType descending
                select tool.Activate()
            ).ToCompositeDisposable();            
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
