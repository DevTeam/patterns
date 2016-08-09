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
        private readonly IProperty _toolProperty;

        private readonly IDisposable _disposable;

        public Session(
            IResolver<ISession, ITool> toolResolver,
            [State] IEnumerable<IPropertyValue> properties,
            [Dependency(Key = WellknownProperty.Tool)] IProperty toolProperty)
        {
            if (toolResolver == null) throw new ArgumentNullException(nameof(toolResolver));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (toolProperty == null) throw new ArgumentNullException(nameof(toolProperty));

            _toolProperty = toolProperty;
            Properties = new ReadOnlyCollection<IPropertyValue>(new List<IPropertyValue>(properties));
            _disposable = (
                from toolName in GetToolNames()
                let toolId = (WellknownTool)Enum.Parse(typeof(WellknownTool), toolName)
                let tool = toolResolver.Resolve(this, toolId)
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
            return Properties.Where(p => Equals(p.Property, _toolProperty)).Select(i => i.Value);
        }
    }
}
