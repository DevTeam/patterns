namespace DevTeam.Patterns.IoC
{
    using System;

    public class ContainerInfo
    {
        public ContainerInfo(IContainer parentContainer, string name)
        {
            if (parentContainer == null) throw new ArgumentNullException(nameof(parentContainer));
            if (name == null) throw new ArgumentNullException(nameof(name));

            ParentContainer = parentContainer;
            Name = name;
        }

        public IContainer ParentContainer { get; private set; }

        public string Name { get; private set; }
    }
}
