namespace DevTeam.Patterns.IoC
{
    using System;

    public class ContainerDescription
    {
        public ContainerDescription(IContainer parentContainer, object key)
        {
            if (parentContainer == null) throw new ArgumentNullException(nameof(parentContainer));

            ParentContainer = parentContainer;
            Key = key;
        }

        public IContainer ParentContainer { get; private set; }

        public object Key { get; private set; }

        public override string ToString()
        {
            return $"{nameof(ContainerDescription)} [ParentContainer: {ParentContainer}, Key: {Key?.ToString() ?? "null"}]";
        }
    }
}
