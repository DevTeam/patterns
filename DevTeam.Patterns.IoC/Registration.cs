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

        public Registration(Type stateType, Type contractType, object key)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            StateType = stateType;
            ContractType = contractType;
            Key = key;
            _resources = Disposable.Empty();

            unchecked
            {
                _hashCode = StateType.GetHashCode();
                _hashCode = (_hashCode * 397) ^ ContractType.GetHashCode();
                _hashCode = (_hashCode * 397) ^ (Key?.GetHashCode() ?? 0);
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

        public Type StateType { get; }

        public Type ContractType { get; }

        public object Key { get; }

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
            return ContractType == other.ContractType && Equals(Key, other.Key) && StateType == other.StateType;
        }

        public void Dispose()
        {
            _resources.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(Registration)} [StateType: {StateType.Name}, ContractType: {ContractType.Name}, Key: {Key?.ToString() ?? "null"}]";
        }
    }
}
