namespace DevTeam.Patterns.IoC
{
    using System;

    internal class GenericKey: IKey, IDisposable
    {
        private readonly KeyDescription _description;
        
        public GenericKey(KeyDescription description)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));

            _description = description;
        }

        public Type StateType => _description.StateType;

        public Type InstanceType
        {
            get
            {
                if (_description.InstanceType.GenericTypeArguments.Length == 0)
                {
                    return _description.InstanceType;
                }

                return _description.InstanceType.GetGenericTypeDefinition();
            }
        }

        public string Name => _description.Name;

        public void Dispose()
        {
            _description.Resources.Dispose();
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