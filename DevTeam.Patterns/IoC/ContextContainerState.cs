namespace DevTeam.Patterns.IoC
{
    using System;

    public class ContextContainerState
    {        
        public ContextContainerState(IContainer targetContainer, IContext context)
        {
            if (targetContainer == null) throw new ArgumentNullException(nameof(targetContainer));
            if (context == null) throw new ArgumentNullException(nameof(context));

            TargetContainer = targetContainer;
            Context = context;
        }

        public IContainer TargetContainer { get; }

        public IContext Context { get; }
    }
}
