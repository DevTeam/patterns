namespace DevTeam.TestTool.Engine.Host
{
    using System;

    using Patterns.IoC;
    using Contracts;

    using Patterns.Dispose;

    internal class ToolResolver: IResolver<ISession, ITool>
    {
        private readonly IResolver<IContainer> _containerResolver;
        private readonly IResolver<IConfiguration> _configurationResolver;

        public ToolResolver(
            IResolver<IContainer> containerResolver,
            IResolver<IConfiguration> configurationResolver)
        {
            if (containerResolver == null) throw new ArgumentNullException(nameof(containerResolver));
            if (configurationResolver == null) throw new ArgumentNullException(nameof(configurationResolver));

            _containerResolver = containerResolver;
            _configurationResolver = configurationResolver;
        }

        public ITool Resolve(ISession session, object toolName = null)
        {
            if (toolName == null) throw new ArgumentNullException(nameof(toolName));
            var tooContainerConfiguration = _configurationResolver.Resolve(toolName);
            var tooContainer = _containerResolver.Resolve(toolName);
            tooContainerConfiguration.Apply(tooContainer).ToCompositeDisposable();
            return tooContainer.Resolve<ISession, ITool>(session);
        }
    }
}
