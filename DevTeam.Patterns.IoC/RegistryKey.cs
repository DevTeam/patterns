namespace DevTeam.Patterns.IoC
{
    using System;

    internal class RegistryKey: IRegistryKey, IDisposable
    {
        private readonly IDisposable _resources;

        public RegistryKey(Type stateType, Type instanceType, string name, IDisposable resources)
        {            
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (resources == null) throw new ArgumentNullException(nameof(resources));

            StateType = stateType;
            InstanceType = instanceType;
            Name = name;
            _resources = resources;
        }

        public Type StateType { get; private set; }

        public Type InstanceType { get; private set; }

        public string Name { get; private set; }

        public void Dispose()
        {
            _resources.Dispose();
        }

        public override string ToString()
        {
            return $"{InstanceType.FullName}({StateType.FullName}, \"{Name}\")";
        }

        public bool Equals(IRegistryKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StateType == other.StateType && InstanceType == other.InstanceType && String.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RegistryKey)obj);
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