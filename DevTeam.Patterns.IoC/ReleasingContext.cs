namespace DevTeam.Patterns.IoC
{
    using System;

    internal class ReleasingContext : IReleasingContext
    {
        public ReleasingContext(IContainer container, IRegestryKey regestryKey, string name)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (regestryKey == null) throw new ArgumentNullException(nameof(regestryKey));
            if (name == null) throw new ArgumentNullException(nameof(name));

            Container = container;
            RegestryKey = regestryKey;
            Name = name;
        }

        public IContainer Container { get; }

        public IRegestryKey RegestryKey { get; }

        public string Name { get; }
    }
}
