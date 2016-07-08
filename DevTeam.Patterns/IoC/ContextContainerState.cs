namespace DevTeam.Patterns.IoC
{
    using System;

    public class ContextContainerState
    {        
        public ContextContainerState(IContainer targetContainer, Func<IContext> contextFactory)
        {
            if (targetContainer == null) throw new ArgumentNullException(nameof(targetContainer));
            if (contextFactory == null) throw new ArgumentNullException(nameof(contextFactory));

            TargetContainer = targetContainer;
            ContextFactory = contextFactory;
        }

        public IContainer TargetContainer { get; }

        public Func<IContext> ContextFactory { get; }
    }
}
