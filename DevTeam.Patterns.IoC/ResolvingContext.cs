namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ResolvingContext: IResolvingContext, IDisposable
    {
        private static Guid? _resolvingId;
        [ThreadStatic] private static Guid? _perThreadResolvingId;
        private readonly Guid? _prevResolvingId;
        private readonly Guid? _prevPerThreadResolvingId;

        public ResolvingContext(IResolver resolver, IRegistration registration, Type resolvingContractType, object state)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (registration == null) throw new ArgumentNullException(nameof(registration));            
            if (resolvingContractType == null) throw new ArgumentNullException(nameof(resolvingContractType));

            Resolver = resolver;
            Registration = registration;            
            ResolvingContractType = resolvingContractType;
            State = state;

            _prevResolvingId = _resolvingId;
            if (_resolvingId == null)
            {
                _resolvingId = Guid.NewGuid();
            }

            _prevPerThreadResolvingId = _perThreadResolvingId;
            if (_perThreadResolvingId == null)
            {
                _perThreadResolvingId = Guid.NewGuid();
            }
        }

        public Guid ResolvingId => _resolvingId ?? Guid.Empty;

        public Guid PerThreadResolvingId => _perThreadResolvingId ?? Guid.Empty;

        public IRegistration Registration { get; }

        public IResolver Resolver { get; }

        public Type ResolvingContractType { get; }

        public object State { get; }

        public void Dispose()
        {
            _resolvingId = _prevResolvingId;
            _perThreadResolvingId = _prevPerThreadResolvingId;
        }
    }
}