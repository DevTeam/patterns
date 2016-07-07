namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct RegistrationDescription
    {
        public RegistrationDescription(Type stateType, Type instanceType, IComparable key, IDisposable resources)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (resources == null) throw new ArgumentNullException(nameof(resources));

            StateType = stateType;
            InstanceType = instanceType;
            Key = key;
            Resources = resources;
        }

        public Type StateType { get; }

        public Type InstanceType { get; }

        public IComparable Key { get; }

        public IDisposable Resources { get; }
    }
}
