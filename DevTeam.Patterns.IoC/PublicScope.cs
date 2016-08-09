namespace DevTeam.Patterns.IoC
{
    using System;

    internal class PublicScope : IScope
    {
        public PublicScope()
        {
        }

        public PublicScope(IContainer owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
        }

        public bool ReadyToRegister(bool isRoot, IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return true;
        }

        public bool ReadyToResolve(bool isRoot, IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return true;
        }
    }
}
