namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct RegistrationDescription
    {
        public RegistrationDescription(Type stateType, Type contractType, object key, IDisposable resources)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (resources == null) throw new ArgumentNullException(nameof(resources));

            StateType = stateType;
            ContractType = contractType;
            Key = key;
            Resources = resources;
        }

        public Type StateType { get; }

        public Type ContractType { get; }

        public object Key { get; }

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
                var hashCode = StateType.GetHashCode();
                hashCode = (hashCode * 397) ^ ContractType.GetHashCode();
                hashCode = (hashCode * 397) ^ (Key?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        private bool Equals(RegistrationDescription other)
        {
            return StateType == other.StateType && ContractType == other.ContractType && Equals(Key, other.Key);
        }
    }
}
