namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    internal struct Registration: IRegistration
    {
        private readonly IDisposable _resources;
        private readonly int _hashCode;
        private readonly Type _stateType;
        private readonly Type _contractType;
        private readonly object _key;
        private readonly bool _isGenericType;
        private readonly Type _genericType;

        public Registration(Type stateType, Type contractType, object key, IDisposable resources = null)
        {
            // Optimize perfomance
            // if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            // if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            _stateType = stateType;
            _contractType = contractType;
            _key = key;
            _resources = resources;
            _isGenericType = contractType.GenericTypeArguments.Length > 0;
            _genericType = _isGenericType ? contractType.GetGenericTypeDefinition() : null;

            unchecked
            {
                _hashCode = stateType.GetHashCode();
                _hashCode = (_hashCode * 397) ^ contractType.GetHashCode();
                _hashCode = (_hashCode * 397) ^ (key?.GetHashCode() ?? 0);
            }
        }

        public Registration(IRegistration registration, IDisposable resources)
            : this(registration.StateType, registration.ContractType, registration.Key, resources)
        {
        }

        public IEnumerable<IRegistration> GetResolveVariants()
        {
            // Strict
            yield return this;

            // For generic type
            if(_isGenericType)
            {
                yield return new Registration(StateType, _genericType, Key);
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
