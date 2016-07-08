namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ResolvingContext: IResolvingContext, IDisposable
    {
        private static Guid? _resolvingId;
        [ThreadStatic] private static Guid? _perThreadResolvingId;
        private readonly Guid? _prevResolvingId;
        private readonly Guid? _prevPerThreadResolvingId;

        public ResolvingContext(IContainer resolvingContainer, IRegistration registration, Type resolvingInstanceType, object state)
        {
            if (resolvingContainer == null) throw new ArgumentNullException(nameof(resolvingContainer));
            if (registration == null) throw new ArgumentNullException(nameof(registration));            
            if (resolvingInstanceType == null) throw new ArgumentNullException(nameof(resolvingInstanceType));

            ResolvingContainer = resolvingContainer;
            Registration = registration;            
            ResolvingInstanceType = resolvingInstanceType;
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

        public IContainer ResolvingContainer { get; }

        public Type ResolvingInstanceType { get; }

        public object State { get; }

        public void Dispose()
        {
            _resolvingId = _prevResolvingId;
            _perThreadResolvingId = _prevPerThreadResolvingId;
        }
    }
}