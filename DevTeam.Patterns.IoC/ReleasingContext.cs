namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ReleasingContext : IReleasingContext
    {
        public ReleasingContext(IRegistration registration)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            Registration = registration;
        }

        public IRegistration Registration { get; }

        public override string ToString()
        {
            return $"{nameof(ReleasingContext)} [Registration: {Registration}]";
        }
    }
}
