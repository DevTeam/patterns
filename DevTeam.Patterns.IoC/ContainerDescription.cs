namespace DevTeam.Patterns.IoC
{
    using System;

    public class ContainerDescription
    {
        public ContainerDescription(IContainer parentContainer, IComparable key)
        {
            if (parentContainer == null) throw new ArgumentNullException(nameof(parentContainer));            

            ParentContainer = parentContainer;
            Key = key;
        }

        public IContainer ParentContainer { get; private set; }

        public IComparable Key { get; private set; }
    }
}
