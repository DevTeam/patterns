using System.Diagnostics.CodeAnalysis;

namespace DevTeam.Patterns.IoC
{
    using System;

    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    internal struct RegistrationDescription
    {
        private readonly Type _stateType;
        private readonly Type _contractType;
        private readonly object _key;

        public RegistrationDescription(Type stateType, Type contractType, object key, IDisposable resources)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (resources == null) throw new ArgumentNullException(nameof(resources));

            _stateType = stateType;
            _contractType = contractType;
            _key = key;
            Resources = resources;
        }

        public Type StateType => _stateType;

        public Type ContractType => _contractType;

        public object Key => _key;

        public IDisposable Resources { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RegistrationDescription && Equals((RegistrationDescription)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _stateType.GetHashCode();
                hashCode = (hashCode * 397) ^ _contractType.GetHashCode();
                hashCode = (hashCode * 397) ^ (_key?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(RegistrationDescription)} [StateType: {StateType.Name}, ContractType: {ContractType.Name}, Key: {Key?.ToString() ?? "null"}]";
        }

        private bool Equals(RegistrationDescription other)
        {
            return _stateType == other._stateType && _contractType == other._contractType && Equals(_key, other._key);
        }
    }
}
