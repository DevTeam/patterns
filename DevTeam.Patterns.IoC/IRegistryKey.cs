namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistryKey: IEquatable<IRegistryKey>
    {
        Type StateType { get; }

        Type InstanceType { get; }

        string Name { get; }
    }
}
