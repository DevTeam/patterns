namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct StrictRegistration: IRegistration
    {
        private readonly RegistrationDescription _description;
        
        public StrictRegistration(RegistrationDescription description)
        {
            _description = description;
        }

        public Type StateType => _description.StateType;

        public Type ContractType => _description.ContractType;

        public object Key => _description.Key;

        public void Dispose()
        {
            _description.Resources.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(StrictRegistration)} [StateType: {StateType.Name}, ContractType: {ContractType.Name}, Key: {Key?.ToString() ?? "null"})";
        }

        public bool Equals(IRegistration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StateType == other.StateType && ContractType == other.ContractType && Equals(Key, other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IRegistration)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StateType.GetHashCode();
                hashCode = (hashCode * 397) ^ ContractType.GetHashCode();
                hashCode = (hashCode * 397) ^ Key?.GetHashCode() ?? 0;
                return hashCode;
            }
        }
    }
}