namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct InternalScope : IScope
    {
        private readonly IContainer _owner;

        public InternalScope(IContainer owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            _owner = owner;
        }

        public bool ReadyToRegister(bool isRoot, IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return true;
        }

        public bool ReadyToResolve(bool isRoot, IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return Equals(_owner, container);
        }
    }
}