namespace DevTeam.Patterns.IoC
{
    using System;

    internal class GenericRegestryKey: IRegestryKey, IDisposable
    {
        private readonly KeyDescription _description;

        public static bool TryCreate(KeyDescription description, out IRegestryKey key)
        {
            if (description.InstanceType.GenericTypeArguments.Length > 0)
            {
                key = new GenericRegestryKey(description);
                return true;
            }

            key = null;
            return false;
        }

        private GenericRegestryKey(KeyDescription description)
        {
            _description = description;            
        }

        public Type StateType => _description.StateType;

        public Type InstanceType => _description.InstanceType.GetGenericTypeDefinition();

        public string Name => _description.Name;

        public void Dispose()
        {
            _description.Resources.Dispose();
        }

        public override string ToString()
        {
            return $"{InstanceType.FullName}({StateType.FullName}, \"{Name}\")";
        }

        public bool Equals(IRegestryKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StateType == other.StateType && InstanceType == other.InstanceType && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IRegestryKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StateType.GetHashCode();
                hashCode = (hashCode * 397) ^ InstanceType.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                return hashCode;
            }
        }
    }
}