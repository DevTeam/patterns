namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IKey: IEquatable<IKey>
    {
        Type StateType { get; }

        Type InstanceType { get; }

        string Name { get; }
    }
}
