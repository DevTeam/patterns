namespace DevTeam.Patterns.IoC
{
    using System;

    internal class GenericRegistration: IRegistration
    {
        private readonly RegistrationDescription _description;

        public static bool TryCreate(RegistrationDescription description, out IRegistration registration)
        {
            if (description.InstanceType.GenericTypeArguments.Length > 0)
            {
                registration = new GenericRegistration(description);
                return true;
            }

            registration = null;
            return false;
        }

        private GenericRegistration(RegistrationDescription description)
        {
            _description = description;            
        }

        public Type StateType => _description.StateType;

        public Type InstanceType => _description.InstanceType.GetGenericTypeDefinition();

        public IComparable Name => _description.Name;

        public void Dispose()
        {
            _description.Resources.Dispose();
        }

        public override string ToString()
        {
            return $"{InstanceType.FullName}({StateType.FullName}, \"{Name}\")";
        }

        public bool Equals(IRegistration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StateType == other.StateType && InstanceType == other.InstanceType && Equals(Name, other.Name);
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
                hashCode = (hashCode * 397) ^ Name?.GetHashCode() ?? 0;
                return hashCode;
            }
        }
    }
}