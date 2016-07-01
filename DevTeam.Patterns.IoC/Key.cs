namespace DevTeam.Patterns.IoC
{
    using System;

    internal class Key: IKey, IDisposable
    {
        private readonly bool _resolve;
        private readonly IDisposable _resources;
        private readonly Type _instanceType;

        public Key(bool resolve, Type stateType, Type instanceType, string name, IDisposable resources)
        {            
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (resources == null) throw new ArgumentNullException(nameof(resources));

            StateType = stateType;
            _instanceType = instanceType;
            Name = name;
            _resolve = resolve;
            _resources = resources;
        }

        public Type StateType { get; }

        public Type InstanceType => _instanceType;

        public Type ResolvingInstanceType
        {
            get
            {
                if (!_resolve || _instanceType.GenericTypeArguments.Length == 0)
                {
                    return _instanceType;
                }

                return _instanceType.GetGenericTypeDefinition();                
            }
        }

        public string Name { get; }

        public void Dispose()
        {
            _resources.Dispose();
        }

        public override string ToString()
        {
            return $"{InstanceType.FullName}({StateType.FullName}, \"{Name}\")";
        }

        public bool Equals(IKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StateType == other.StateType && ResolvingInstanceType == other.ResolvingInstanceType && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Key)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StateType.GetHashCode();
                hashCode = (hashCode * 397) ^ ResolvingInstanceType.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                return hashCode;
            }
        }
    }
}