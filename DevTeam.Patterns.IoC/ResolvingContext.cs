namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct ResolvingContext : IResolvingContext, IDisposable
    {
        private static readonly object LockObject = new object();
        private static long _resolvingId;
        [ThreadStatic]
        private static long _perThreadResolvingId;
        private readonly long _prevResolvingId;
        private readonly long _prevPerThreadResolvingId;

        public ResolvingContext(IContainer registerContainer, IContainer resolveContainer, IRegistration registration, Type resolvingContractType, object state)
        {
            // Optimize perfomance
            // if (registerContainer == null) throw new ArgumentNullException(nameof(registerContainer));
            // if (ResolveContainer == null) throw new ArgumentNullException(nameof(ResolveContainer));
            // if (registration == null) throw new ArgumentNullException(nameof(registration));
            // if (resolvingContractType == null) throw new ArgumentNullException(nameof(resolvingContractType));

            RegisterContainer = registerContainer;
            ResolveContainer = resolveContainer;
            Registration = registration;
            ResolvingContractType = resolvingContractType;
            State = state;

            lock (LockObject)
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

        public IContainer ResolveContainer { get; }

        public Type ResolvingContractType { get; }

        public object State { get; }

        public void Dispose()
        {
            _resolvingId = _prevResolvingId;
            _perThreadResolvingId = _prevPerThreadResolvingId;
        }

        public override string ToString()
        {
            return $"{nameof(ResolvingContext)} [ResolvingId: {ResolvingId}, PerThreadResolvingId: {PerThreadResolvingId}, Registration: {Registration}, RegisterContainer: {RegisterContainer}, ResolveContainer: {ResolveContainer}, ResolvingContractType: {ResolvingContractType.Name}, State: {State ?? "null"}]";
        }
    }
}