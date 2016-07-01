namespace DevTeam.Patterns.IoC
{
    using System;

    public class KeyDescription
    {
        public KeyDescription(Type stateType, Type instanceType, string name, IDisposable resources)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (resources == null) throw new ArgumentNullException(nameof(resources));

            StateType = stateType;
            InstanceType = instanceType;
            Name = name;
            Resources = resources;
        }

        public Type StateType { get; }

        public Type InstanceType { get; }

        public string Name { get; }

        public IDisposable Resources { get; }
    }
}
