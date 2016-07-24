namespace DevTeam.Patterns.IoC
{
    using System;

    public class ContainerDescription
    {
        public ContainerDescription(IResolver parentResolver, object key)
        {
            if (parentResolver == null) throw new ArgumentNullException(nameof(parentResolver));            

            ParentResolver = parentResolver;
            Key = key;
        }

        public IResolver ParentResolver { get; private set; }

        public object Key { get; private set; }
    }
}
