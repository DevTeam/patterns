namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ResolvingContext: IResolvingContext
    {
        public ResolvingContext(IContainer resolvingContainer, IRegistration registration, Type resolvingInstanceType, object state)
        {
            if (resolvingContainer == null) throw new ArgumentNullException(nameof(resolvingContainer));
            if (registration == null) throw new ArgumentNullException(nameof(registration));            
            if (resolvingInstanceType == null) throw new ArgumentNullException(nameof(resolvingInstanceType));

            ResolvingContainer = resolvingContainer;
            Registration = registration;            
            ResolvingInstanceType = resolvingInstanceType;
            State = state;
        }

        public IRegistration Registration { get; }

        public IContainer ResolvingContainer { get; }

        public Type ResolvingInstanceType { get; }

        public object State { get; }
    }
}
