namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ResolvingContext : IResolvingContext
    {
        public ResolvingContext(IContainer container, IRegistration registration, Type resolvingInstanceType, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registration == null) throw new ArgumentNullException(nameof(registration));            
            if (resolvingInstanceType == null) throw new ArgumentNullException(nameof(resolvingInstanceType));

            Container = container;
            Registration = registration;            
            ResolvingInstanceType = resolvingInstanceType;
            State = state;
        }

        public IContainer Container { get; }

        public IRegistration Registration { get; }        

        public Type ResolvingInstanceType { get; }

        public object State { get; }
    }
}
