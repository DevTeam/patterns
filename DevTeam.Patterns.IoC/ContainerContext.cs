namespace DevTeam.Patterns.IoC
{
    using System;

    public class ContainerContext: IContainerContext
    {
        public ContainerContext(IContainer targetContainer)
        {
            if (targetContainer == null) throw new ArgumentNullException(nameof(targetContainer));

            TargetContainer = targetContainer;
        }

        public IContainer TargetContainer { get; }
    }
}
