namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ResolvingContext : IResolvingContext, IDisposable
    {
        private static object _lockObject = new object();
        private static long _resolvingId;
        [ThreadStatic]
        private static long _perThreadResolvingId;
        private readonly long _prevResolvingId;
        private readonly long _prevPerThreadResolvingId;

        public ResolvingContext(IContainer registerContainer, IContainer resolverContainer, IRegistration registration, Type resolvingContractType, object state)
        {
            if (registerContainer == null) throw new ArgumentNullException(nameof(registerContainer));
            if (resolverContainer == null) throw new ArgumentNullException(nameof(resolverContainer));
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (resolvingContractType == null) throw new ArgumentNullException(nameof(resolvingContractType));

            RegisterContainer = registerContainer;
            ResolverContainer = resolverContainer;
            Registration = registration;
            ResolvingContractType = resolvingContractType;
            State = state;

            lock (_lockObject)
            {
                _prevResolvingId = _resolvingId;
                if (_resolvingId == 0)
                {
                    _resolvingId++;
                }
            }

            _prevPerThreadResolvingId = _perThreadResolvingId;
            if (_perThreadResolvingId == 0)
            {
                _perThreadResolvingId++;
            }
        }

        public long ResolvingId => _resolvingId;

        public long PerThreadResolvingId => _perThreadResolvingId;

        public IRegistration Registration { get; }

        public IContainer RegisterContainer { get; }

        public IContainer ResolverContainer { get; }

        public Type ResolvingContractType { get; }

        public object State { get; }

        public void Dispose()
        {
            _resolvingId = _prevResolvingId;
            _perThreadResolvingId = _prevPerThreadResolvingId;
        }

        public override string ToString()
        {
            return $"{nameof(ResolvingContext)} [ResolvingId: {ResolvingId}, PerThreadResolvingId: {PerThreadResolvingId}, Registration: {Registration}, RegisterContainer: {RegisterContainer}, ResolverContainer: {ResolverContainer}, ResolvingContractType: {ResolvingContractType.Name}, State: {State ?? "null"}]";
        }
    }
}