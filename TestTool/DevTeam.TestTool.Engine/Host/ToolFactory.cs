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

        public ITool Create(ISession session)
        {
            var toolProperty = session.Properties.SingleOrDefault(p => Equals(p.Property, ToolProperty.Shared));
            var tooContainerConfiguration = _container.Resolve<IConfiguration>(toolProperty.Value);
            tooContainerConfiguration.Apply(_container);
            return _container.Resolve<ISession, ITool>(session);
        }
    }
}
