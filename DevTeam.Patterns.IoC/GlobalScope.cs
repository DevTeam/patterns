namespace DevTeam.Patterns.IoC
{
    using System;

    internal class GlobalScope : IScope
    {
        public GlobalScope(IContainer owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
        }

        public bool ReadyToRegister(bool isRoot, IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return isRoot;
        }

        public bool ReadyToResolve(bool isRoot, IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return isRoot;
        }
    }
}
