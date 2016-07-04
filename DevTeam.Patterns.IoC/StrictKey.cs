namespace DevTeam.Patterns.IoC
{
    using System;

    internal class StrictKey: IKey, IDisposable
    {
        private readonly KeyDescription _keyDescription;
        
        public StrictKey(KeyDescription keyDescription)
        {
            if (keyDescription == null) throw new ArgumentNullException(nameof(keyDescription));

            _keyDescription = keyDescription;
        }

        public Type StateType => _keyDescription.StateType;

        public Type InstanceType => _keyDescription.InstanceType;

        public string Name => _keyDescription.Name;

        public void Dispose()
        {
            _keyDescription.Resources.Dispose();
        }

        public override string ToString()
        {
            return $"{InstanceType.FullName}({StateType.FullName}, \"{Name}\")";
        }

        public bool Equals(IKey other)
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
            return Equals((StrictKey)obj);
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