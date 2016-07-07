namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ReleasingContext : IReleasingContext
    {
        public ReleasingContext(IContainer container, IRegistration registration, string name)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (name == null) throw new ArgumentNullException(nameof(name));

            Container = container;
            Registration = registration;
            Name = name;
        }

        public IContainer Container { get; }

        public IRegistration Registration { get; }

        public string Name { get; }
    }
}
