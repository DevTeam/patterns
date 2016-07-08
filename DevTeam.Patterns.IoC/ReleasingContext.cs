namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ReleasingContext : IReleasingContext
    {
        public ReleasingContext(IContainer container, IRegistration registration, object key)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            Container = container;
            Registration = registration;
            Key = key;
        }

        public IContainer Container { get; }

        public IRegistration Registration { get; }

        public object Key { get; }
    }
}
