namespace DevTeam.Patterns.IoC
{
    using System;

    internal class StrictRegistration: IRegistration
    {
        private readonly RegistrationDescription _description;
        
        public StrictRegistration(RegistrationDescription description)
        {
            _description = description;
        }

        public Type StateType => _description.StateType;

        public Type InstanceType => _description.InstanceType;

        public object Key => _description.Key;

        public void Dispose()
        {
            _description.Resources.Dispose();
        }

        public override string ToString()
        {
            return $"{InstanceType.FullName}({StateType.FullName}, \"{Key}\")";
        }

        public bool Equals(IRegistration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StateType == other.StateType && InstanceType == other.InstanceType && Equals(Key, other.Key);
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
                hashCode = (hashCode * 397) ^ InstanceType.GetHashCode();
                hashCode = (hashCode * 397) ^ Key?.GetHashCode() ?? 0;
                return hashCode;
            }
        }
    }
}