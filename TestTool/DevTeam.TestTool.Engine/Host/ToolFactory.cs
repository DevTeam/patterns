namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Linq;

    using Contracts;

    using Patterns.IoC;

    internal class ToolFactory : IToolFactory
    {
        private readonly IContainer _container;

        public ToolFactory(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
        }

        public ITool Create(ISession session, string toolName = "")
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (toolName == null) throw new ArgumentNullException(nameof(toolName));

            if (string.IsNullOrEmpty(toolName))
            {
                var toolProperty = session.Properties.SingleOrDefault(p => Equals(p.Property, ToolProperty.Shared));
                toolName = toolProperty.Value;
            }

            var tooContainerConfiguration = _container.Resolve<IConfiguration>(toolName);
            var toolContainer = tooContainerConfiguration.Apply(_container);
            return toolContainer.Resolve<ISession, ITool>(session);
        }
    }
}
