namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Dispose;

    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    internal struct Registration: IRegistration
    {
        private IDisposable _resources;
        private readonly int _hashCode;
        private readonly Type _stateType;
        private readonly Type _contractType;
        private readonly object _key;

        public Registration(Type stateType, Type contractType, object key)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            _stateType = stateType;
            _contractType = contractType;
            _key = key;
            _resources = Disposable.Empty();

            unchecked
            {
                _hashCode = stateType.GetHashCode();
                _hashCode = (_hashCode * 397) ^ contractType.GetHashCode();
                _hashCode = (_hashCode * 397) ^ (key?.GetHashCode() ?? 0);
            }
        }

        public static IRegistration CreateFromRegistration(IRegistration registration, IDisposable resources)
        {
            return new Registration(registration.StateType, registration.ContractType, registration.Key) { _resources = resources };
        }

        public static IEnumerable<IRegistration> CreateRegistrationVariants(IRegistration registration)
        {
            yield return registration;
            if (registration.ContractType.GenericTypeArguments.Length > 0)
            {
                yield return new Registration(registration.StateType, registration.ContractType.GetGenericTypeDefinition(), registration.Key);
            }
        }

        public Type StateType => _stateType;

        public Type ContractType => _contractType;

        public object Key => _key;

        public bool Equals(Registration other)
        {
            return Equals((IRegistration)other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IRegistration && Equals((IRegistration)obj);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(IRegistration other)
        {
            return _contractType == other.ContractType && Equals(_key, other.Key) && _stateType == other.StateType;
        }

        public void Dispose()
        {
            _resources?.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(Registration)} [StateType: {StateType.Name}, ContractType: {ContractType.Name}, Key: {Key?.ToString() ?? "null"}]";
        }
    }
}
