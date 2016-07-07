namespace DevTeam.Patterns.IoC
{
    using System;

    public class ContainerDescription
    {
        public ContainerDescription(IContainer parentContainer, IComparable name)
        {
            if (parentContainer == null) throw new ArgumentNullException(nameof(parentContainer));            

            ParentContainer = parentContainer;
            Name = name;
        }

        public IContainer ParentContainer { get; private set; }

        public IComparable Name { get; private set; }
    }
}
